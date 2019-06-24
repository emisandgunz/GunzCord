using GunzSharp.Commands;
using GunzSharp.Packet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace GunzSharp.Test
{
	public class MCommandTests
	{
		[Fact]
		public void TestDecodeServerList()
		{
			byte[] data = new byte[]
			{
				0x64, 0x00, 0x77, 0x00, 0xd2, 0x17, 0x71, 0x00, 0x42, 0x9c, 0x00, 0x68, 0x00, 0x00, 0x00, 0x30,
				0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x48, 0x65, 0x72, 0x6f, 0x47, 0x61, 0x6d, 0x65, 0x72,
				0x73, 0x20, 0x51, 0x75, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc6, 0xfb, 0x50, 0x66, 0xd4, 0x17, 0x3d, 0x0b, 0x15,
				0x00, 0xd0, 0x07, 0x3a, 0x00, 0x00, 0x00, 0x48, 0x65, 0x72, 0x6f, 0x47, 0x61, 0x6d, 0x65, 0x72,
				0x73, 0x20, 0x43, 0x6c, 0x61, 0x6e, 0x20, 0x57, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc6, 0xfb, 0x50, 0x66, 0x70, 0x17, 0xd9, 0x0a, 0x0d,
				0x00, 0xdc, 0x05, 0x31, 0x00, 0x00, 0x00
			};

			InitMClient();
			MClient.Instance.CommandBuilder.MakeCommand(data);

			Assert.Single(MClient.Instance.CommandBuilder.CommandList);

			MCommand command = MClient.Instance.CommandBuilder.GetCommand();

			Assert.Equal((int)MSharedCommand.MC_RESPONSE_SERVER_LIST_INFO, command.CommandDesc.ID);
			Assert.Empty(MClient.Instance.CommandBuilder.CommandList);
		}

		[Fact]
		public void TestLoginRequest()
		{
			byte[] dataConnect = new byte[]
			{
				0x0a, 0x00, 0x1a, 0x00, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0xdd, 0x99, 0x02, 0x00, 0xf0, 0xd8, 0x1d, 0x90
			};

			InitMClient();
			MClient.Instance.CommandBuilder.MakeCommand(dataConnect);
			Assert.Single(MClient.Instance.CommandBuilder.NetCmdList);

			byte[] netCmd = MClient.Instance.CommandBuilder.GetNetCommand();

			Assert.Empty(MClient.Instance.CommandBuilder.NetCmdList);
			Assert.Equal(PacketConsts.REPLY_CONNECT_MSG_SIZE, netCmd.Length);

			IntPtr msgPtr = Marshal.AllocHGlobal(PacketConsts.REPLY_CONNECT_MSG_SIZE);
			Marshal.Copy(netCmd, 0, msgPtr, PacketConsts.REPLY_CONNECT_MSG_SIZE);
			MReplyConnectMsg replyConnectMsg = Marshal.PtrToStructure<MReplyConnectMsg>(msgPtr);
			Marshal.FreeHGlobal(msgPtr);

			Assert.Equal(2U, replyConnectMsg.HostLow);

			MPacketCrypterKey key = new MPacketCrypterKey();
			key.InitKey();

			MUID serverID = new MUID(replyConnectMsg.HostHigh, replyConnectMsg.HostLow);
			MUID clientID = new MUID(replyConnectMsg.AllocHigh, replyConnectMsg.AllocLow);
			MPacketCrypter.MakeSeedKey(ref key, serverID, clientID, replyConnectMsg.TimeStamp);
			MClient.Instance.ServerPacketCrypter.InitKey(ref key);
			MClient.Instance.CommandBuilder.InitCrypt(MClient.Instance.ServerPacketCrypter, false);
		}

		private void InitMClient()
		{
			if (MClient.Instance.CommandManager.GetCommandDescCount() == 0)
			{
				MSharedCommandTable.RegisterCommands(MClient.Instance.CommandManager, MSharedCommandType.Client);
			}

			MClient.Instance.CommandBuilder.Clear();
		}
	}
}
