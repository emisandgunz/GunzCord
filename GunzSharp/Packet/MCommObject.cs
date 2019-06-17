using GunzSharp.Commands;
using GunzSharp.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunzSharp.Packet
{
	public class MCommObject
	{
		public MUID Uid { get; set; }

		public MCommandBuilder CommandBuilder { get; protected set; }
		public MPacketCrypter PacketCrypter { get; protected set; }

		public MCommandCommunicator DirectConnection { get; set; }

		public string IP { get; protected set; }
		public int Port { get; protected set; }
		public uint IPNumber { get; protected set; }
		public bool Allowed { get; set; }

		public bool PassiveSocket { get; set; }

		public MCommObject(MCommandCommunicator communicator)
		{
			Uid = MUID.Empty;

			DirectConnection = null;

			IP = null;
			Port = 0;

			Allowed = true;
		}

	}
}
