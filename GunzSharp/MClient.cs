using GunzSharp.Commands;
using GunzSharp.Packet;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace GunzSharp
{
	public class MClient : MCommandCommunicator
	{
		public static MClient Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new MClient();
				}

				return instance;
			}
		}

		protected object recvLock = new object();

		private static MClient instance;

		protected Socket ClientSocket { get; set; }

		protected MCommandBuilder CommandBuilder { get; set; }

		protected MUID Server { get; set; }

		protected MPacketCrypter ServerPacketCrypter { get; set; }

		private MClient()
		{
			CommandBuilder = new MCommandBuilder(MUID.Empty, MUID.Empty, CommandManager);

			ClientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		}

		public override int Connect(MCommObject commObj)
		{
			throw new NotImplementedException();
		}

		public override void Disconnect(MUID uid)
		{
			throw new NotImplementedException();
		}

		protected int MakeCmdPacket(byte[] buffer, int maxPacketSize, MPacketCrypter packetCrypter, MCommand command)
		{
			MPacketHeader header = new MPacketHeader();
			int packetHeaderSize = Marshal.SizeOf(typeof(MPacketHeader));

			int cmdSize = maxPacketSize - packetHeaderSize;
			byte[] cmdBuffer = new byte[cmdSize];

			header.Checksum = 0;
			int packetSize = 0;

			if (command.CommandDesc.IsFlag(MCommandDescFlags.MCCT_NON_ENCRYPTED))
			{
				header.Msg = PacketConsts.MSGID_RAWCOMMAND;

				cmdSize = command.GetData(ref cmdBuffer, cmdSize);
				packetSize = packetHeaderSize + cmdSize;
				header.Size = (ushort)packetSize;
			}
			else
			{
				if (packetCrypter == null)
				{
					return 0;
				}

				header.Msg = PacketConsts.MSGID_COMMAND;

				cmdSize = command.GetData(ref cmdBuffer, cmdSize);
				packetSize = packetHeaderSize + cmdSize;
				header.Size = (ushort)packetSize;

				byte[] headerSizeBytes = BitConverter.GetBytes(header.Size);

				if (!packetCrypter.Encrypt(headerSizeBytes))
				{
					return 0;
				}

				header.Size = BitConverter.ToUInt16(headerSizeBytes, 0);

				if (!packetCrypter.Encrypt(cmdBuffer))
				{
					return 0;
				}
			}

			IntPtr headerPtr = Marshal.AllocHGlobal(packetHeaderSize);
			Marshal.StructureToPtr(header, headerPtr, true);
			Marshal.Copy(headerPtr, buffer, 0, packetHeaderSize);
			Marshal.FreeHGlobal(headerPtr);

			Buffer.BlockCopy(cmdBuffer, 0, buffer, packetHeaderSize, cmdSize);

			header.Checksum = PacketHelpers.BuildChecksum(buffer);

			byte[] checkSumBytes = BitConverter.GetBytes(header.Checksum);
			Buffer.BlockCopy(cmdBuffer, 0, checkSumBytes, sizeof(uint) * 2, sizeof(uint));

			return packetSize;
		}

		protected override void SendCommand(MCommand command)
		{
			int packetSize = CalcPacketSize(command);
			byte[] buffer = new byte[packetSize];

			int size = MakeCmdPacket(buffer, packetSize, null, command);

			if (size > 0)
			{
				// TODO: Send to socket
			}
		}
	}
}
