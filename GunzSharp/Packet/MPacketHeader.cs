using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GunzSharp.Packet
{
	public static class PacketConsts
	{
		public const int MAX_PACKET_SIZE = 16384;

		public const ushort MSG_COMMAND = 1000;

		public const ushort MSGID_REPLYCONNECT = 10;
		public const ushort MSGID_RAWCOMMAND = 100;
		public const ushort MSGID_COMMAND = 101;

		public const int PACKET_HEADER_SIZE = 6;
		public const int REPLY_CONNECT_MSG_SIZE = 26;
	}

	public static class PacketHelpers
	{
		public static ushort BuildChecksum(byte[] buffer)
		{
			int packetHeaderSize = Marshal.SizeOf(typeof(MPacketHeader));
			int packetSize = Math.Min(ushort.MaxValue, buffer.Length);

			ushort checkSum = 0;

			for (int i = packetHeaderSize; i < packetSize; i++)
			{
				checkSum += buffer[i];
			}

			for (int i = 0; i < 4; i++)
			{
				checkSum -= buffer[i];
			}

			return checkSum;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MPacketHeader
	{
		public ushort Msg { get; set; }
		public ushort Size { get; set; }
		public ushort Checksum { get; set; }

		public int CalcPacketSize(MPacketCrypter crypter)
		{
			ushort packetSize = 0;

			if (Msg == PacketConsts.MSGID_COMMAND)
			{
				if (crypter != null)
				{
					byte[] sizeBytes = BitConverter.GetBytes(Size);
					byte[] packetSizeBytes = BitConverter.GetBytes(packetSize);

					crypter.Decrypt(sizeBytes, packetSizeBytes);
					packetSize = BitConverter.ToUInt16(packetSizeBytes, 0);
				}
			}
			else
			{
				packetSize = Size;
			}

			return (int)packetSize;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MReplyConnectMsg 
	{
		public ushort Msg { get; set; }
		public ushort Size { get; set; }
		public ushort Checksum { get; set; }

		public uint HostHigh { get; set; }
		public uint HostLow { get; set; }
		public uint AllocHigh { get; set; }
		public uint AllocLow { get; set; }
		public uint TimeStamp { get; set; }

		public int CalcPacketSize(MPacketCrypter crypter)
		{
			ushort packetSize = 0;

			if (Msg == PacketConsts.MSGID_COMMAND)
			{
				if (crypter != null)
				{
					byte[] sizeBytes = BitConverter.GetBytes(Size);
					byte[] packetSizeBytes = BitConverter.GetBytes(packetSize);

					crypter.Decrypt(sizeBytes, packetSizeBytes);
					packetSize = BitConverter.ToUInt16(packetSizeBytes, 0);
				}
			}
			else
			{
				packetSize = Size;
			}

			return (int)packetSize;
		}
	}
}
