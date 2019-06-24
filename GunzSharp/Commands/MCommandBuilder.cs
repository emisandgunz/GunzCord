using GunzSharp.Packet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GunzSharp.Commands
{
	public class MCommandBuilder
	{
		public const int COMMAND_BUFFER_LEN = 16384;
		public const int MAX_COMMAND_COUNT_FLOODING = 50;

		protected ulong LastCommandMakeTime { get; set; }
		protected int CommandCountPerSec { get; set; }

		protected MUID UidSender { get; set; }
		protected MUID UidReceiver { get; set; }
		protected MCommandManager CommandManager { get; set; }

		protected byte[] Buffer { get; set; }
		protected int BufferNext { get; set; }

		public LinkedList<MCommand> CommandList { get; protected set; }
		public LinkedList<byte[]> NetCmdList { get; protected set; }

		protected MPacketCrypter PacketCrypter { get; set; }
		protected MCommandSNChecker CommandSNChecker { get; set; }
		protected bool CheckCommandSN { get; set; }

		public MCommandBuilder(MUID uidSender, MUID uidReceiver, MCommandManager cmdMgr)
		{
			PacketCrypter = null;
			UidSender = uidSender;
			UidReceiver = uidReceiver;
			CommandManager = cmdMgr;

			Buffer = new byte[0];
			BufferNext = 0;
			CheckCommandSN = true;

			CommandCountPerSec = 0;
			LastCommandMakeTime = 0;

			CommandList = new LinkedList<MCommand>();
			NetCmdList = new LinkedList<byte[]>();
		}

		public int MakeCommand(byte[] buffer)
		{
			int offset = 0;
			int len = buffer.Length;
			int cmdCount = 0;
			int packetSize = 0;

			MPacketHeader packet;

			IntPtr headerPtr = Marshal.AllocHGlobal(PacketConsts.PACKET_HEADER_SIZE);

			while (len >= PacketConsts.PACKET_HEADER_SIZE)
			{
				byte[] headerBytes = new byte[PacketConsts.PACKET_HEADER_SIZE];
				System.Buffer.BlockCopy(buffer, offset, headerBytes, 0, PacketConsts.PACKET_HEADER_SIZE);

				Marshal.Copy(buffer, offset, headerPtr, PacketConsts.PACKET_HEADER_SIZE);
				packet = Marshal.PtrToStructure<MPacketHeader>(headerPtr);

				packetSize = CalcPacketSize(ref packet);

				if ((packetSize > len || packetSize <= 0))
				{
					break;
				}

				if (packet.Msg == PacketConsts.MSGID_RAWCOMMAND)
				{
					ushort checksum = PacketHelpers.BuildChecksum(buffer);

					if (packet.Checksum != checksum)
					{
						return -1;
					}
					else if (packetSize > PacketConsts.MAX_PACKET_SIZE)
					{
						return -1;
					}
					else
					{
						MCommand cmd = new MCommand();
						int cmdSize = packetSize - PacketConsts.PACKET_HEADER_SIZE;

						byte[] cmdBuffer = new byte[cmdSize];
						System.Buffer.BlockCopy(buffer, offset + PacketConsts.PACKET_HEADER_SIZE, cmdBuffer, 0, cmdSize);

						if (cmd.SetData(cmdBuffer, CommandManager, (ushort)cmdSize))
						{
							if (CheckCommandSN)
							{
								if (!CommandSNChecker.CheckValidate(cmd.SerialNumber))
								{
									return -1;
								}
							}

							cmd.Sender = UidSender;
							cmd.Receiver = UidReceiver;
							CommandList.AddLast(cmd);
						}
						else
						{
							return -1;
						}
					}
				}
				else if (packet.Msg == PacketConsts.MSGID_COMMAND)
				{
					ushort checksum = PacketHelpers.BuildChecksum(headerBytes);

					if (packet.Checksum != checksum)
					{
						return -1;
					}
					else if (packetSize > PacketConsts.MAX_PACKET_SIZE)
					{
						return -1;
					}
					else
					{
						MCommand cmd = new MCommand();
						int cmdSize = packetSize - PacketConsts.PACKET_HEADER_SIZE;

						byte[] cmdBuffer = new byte[cmdSize];
						System.Buffer.BlockCopy(buffer, offset + PacketConsts.PACKET_HEADER_SIZE, cmdBuffer, 0, cmdSize);

						if (PacketCrypter != null)
						{
							if (!PacketCrypter.Decrypt(cmdBuffer))
							{
								return -1;
							}
						}

						if (cmd.SetData(cmdBuffer, CommandManager, (ushort)cmdSize))
						{
							if (CheckCommandSN)
							{
								if (!CommandSNChecker.CheckValidate(cmd.SerialNumber))
								{
									return -1;
								}
							}

							cmd.Sender = UidSender;
							cmd.Receiver = UidReceiver;
							CommandList.AddLast(cmd);
						}
						else
						{
							return -1;
						}
					}
				}
				else if (packet.Msg == PacketConsts.MSGID_REPLYCONNECT)
				{
					if (packetSize == PacketConsts.REPLY_CONNECT_MSG_SIZE)
					{
						byte[] packetBytes = new byte[packetSize];
						System.Buffer.BlockCopy(buffer, offset, packetBytes, 0, packetSize);
						NetCmdList.AddLast(packetBytes);
					}
					else
					{
						return -1;
					}
				}
				else
				{
					return -1;
				}

				offset += packetSize;
				len -= packetSize;
				cmdCount++;
			}

			Marshal.FreeHGlobal(headerPtr);

			return len;
		}

		public void Clear()
		{
			CommandList.Clear();
			NetCmdList.Clear();
		}

		public bool Read(byte[] buffer, bool floodCheck, out bool floodResult)
		{
			floodResult = false;

			MPacketHeader packet;

			IntPtr headerPtr = Marshal.AllocHGlobal(PacketConsts.PACKET_HEADER_SIZE);
			Marshal.Copy(Buffer, 0, headerPtr, PacketConsts.PACKET_HEADER_SIZE);
			packet = Marshal.PtrToStructure<MPacketHeader>(headerPtr);
			Marshal.FreeHGlobal(headerPtr);

			if (CheckBufferEmpty())
			{
				if (buffer.Length < PacketConsts.PACKET_HEADER_SIZE || (buffer.Length < CalcPacketSize(ref packet)))
				{
					AddBuffer(buffer);
				}
				else
				{
					int commandCount = CalcCommandCount(buffer);

					if (CheckFlooding(commandCount) && floodCheck)
					{
						floodResult = true;

						return false;
					}

					int spareData = MakeCommand(buffer);

					if (spareData > 0)
					{
						byte[] spareBuffer = new byte[spareData];
						System.Buffer.BlockCopy(buffer, buffer.Length - spareData, spareBuffer, 0, spareData);

						AddBuffer(spareBuffer);
					}
					else if (spareData < 0)
					{
						return false;
					}
				}
			}
			else
			{
				AddBuffer(buffer);

				if (EstimateBufferToCmd())
				{
					int commandCount = CalcCommandCount(buffer);

					if (CheckFlooding(commandCount) && floodCheck)
					{
						floodResult = true;

						return false;
					}

					int spareData = MakeCommand(buffer);

					if (spareData >= 0)
					{
						MoveBufferToFront(BufferNext - spareData, spareData);
					}
					else
					{
						return false;
					}
				}
			}

			return true;
		}

		public MCommand GetCommand()
		{
			if (CommandList.Count > 0)
			{
				MCommand cmd = CommandList.First.Value;
				CommandList.RemoveFirst();

				return cmd;
			}

			return null;
		}

		public byte[] GetNetCommand()
		{
			if (NetCmdList.Count > 0)
			{
				byte[] cmd = NetCmdList.First.Value;
				NetCmdList.RemoveFirst();

				return cmd;
			}

			return null;
		}

		public void SetUID(ref MUID uidReceiver, ref MUID uidSender)
		{
			UidReceiver = uidReceiver;
			UidSender = uidSender;
		}

		public void InitCrypt(MPacketCrypter packetCrypter, bool checkCommandSerialNumber)
		{
			PacketCrypter = packetCrypter;
			CheckCommandSN = checkCommandSerialNumber;
		}

		protected bool CheckBufferEmpty()
		{
			return BufferNext == 0;
		}

		protected bool EstimateBufferToCmd()
		{
			if (BufferNext < PacketConsts.PACKET_HEADER_SIZE)
			{
				return false;
			}

			MPacketHeader packet;

			IntPtr headerPtr = Marshal.AllocHGlobal(PacketConsts.PACKET_HEADER_SIZE);
			Marshal.Copy(Buffer, 0, headerPtr, PacketConsts.PACKET_HEADER_SIZE);
			packet = Marshal.PtrToStructure<MPacketHeader>(headerPtr);
			Marshal.FreeHGlobal(headerPtr);

			if (BufferNext < CalcPacketSize(ref packet))
			{
				return false;
			}

			return true;
		}

		protected void AddBuffer(byte[] buffer)
		{
			if (buffer == null || buffer.Length <= 0)
			{
				return;
			}

			if ((BufferNext + buffer.Length) >= COMMAND_BUFFER_LEN)
			{
				return;
			}

			System.Buffer.BlockCopy(buffer, 0, Buffer, BufferNext, buffer.Length);
			BufferNext += buffer.Length;
		}

		protected bool MoveBufferToFront(int start, int len)
		{
			if (start + len > BufferNext)
			{
				return false;
			}

			System.Buffer.BlockCopy(Buffer, 0, Buffer, start, len);
			BufferNext = len;

			return true;
		}

		protected int CalcCommandCount(byte[] buffer)
		{
			int offset = 0;
			int len = buffer.Length;
			int cmdCount = 0;
			int packetSize = 0;

			MPacketHeader packet;

			IntPtr headerPtr = Marshal.AllocHGlobal(PacketConsts.PACKET_HEADER_SIZE);

			while (len >= PacketConsts.PACKET_HEADER_SIZE)
			{
				Marshal.Copy(buffer, offset, headerPtr, PacketConsts.PACKET_HEADER_SIZE);
				packet = Marshal.PtrToStructure<MPacketHeader>(headerPtr);

				packetSize = CalcPacketSize(ref packet);

				if ((packetSize > len || packetSize <= 0))
				{
					break;
				}

				offset += packetSize;
				len -= packetSize;
				cmdCount++;
			}

			Marshal.FreeHGlobal(headerPtr);

			return cmdCount;
		}

		protected bool CheckFlooding(int commandCount)
		{
			ulong curTime = (ulong)Environment.TickCount;

			if (curTime - LastCommandMakeTime < 1000)
			{
				CommandCountPerSec += commandCount;

				if (CommandCountPerSec > MAX_COMMAND_COUNT_FLOODING)
				{
					return true;
				}
			}
			else
			{
				CommandCountPerSec = commandCount;
				LastCommandMakeTime = curTime;

				if (CommandCountPerSec > MAX_COMMAND_COUNT_FLOODING)
				{
					return true;
				}
			}

			return false;
		}

		protected int CalcPacketSize(ref MPacketHeader packet)
		{
			return packet.CalcPacketSize(PacketCrypter);
		}
	}
}
