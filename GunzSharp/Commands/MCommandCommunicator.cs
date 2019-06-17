using GunzSharp.Packet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GunzSharp.Commands
{
	public abstract class MCommandCommunicator
	{
		public MCommandManager CommandManager { get; protected set; }

		protected MUID This { get; set; }
		protected MUID DefaultReceiver { get; set; }

		public MCommandCommunicator()
		{
			CommandManager = new MCommandManager();

			This = MUID.Empty;
			DefaultReceiver = MUID.Empty;
		}

		public bool Create()
		{
			OnRegisterCommand(CommandManager);
			return true;
		}

		public abstract int Connect(MCommObject commObj);

		public virtual int OnConnected(ref MUID targetUID, ref MUID allocUID, uint timeStamp, MCommObject commObj)
		{
			This = targetUID;
			DefaultReceiver = allocUID;

			if (commObj != null)
			{
				commObj.CommandBuilder.SetUID(ref allocUID, ref targetUID);
			}

			return 0;
		}

		public virtual bool Post(MCommand command)
		{
			return CommandManager.Post(command);
		}

		public virtual bool Post(MCommand command, out string errorMessage, string message)
		{
			// TODO: implement this
			errorMessage = null;
			return true;
		}

		public MCommand CreateCommand(int cmdID, MUID targetUID)
		{
			return new MCommand(CommandManager.GetCommandDescByID(cmdID), targetUID, This);
		}

		public MCommand GetCommandSafe()
		{
			return CommandManager.GetCommand();
		}

		public void Run()
		{
			// TODO: MGetCheckLoopTimeInstance()->SetPrepareRunTick();
			OnPrepareRun();

			// TODO: MGetCheckLoopTimeInstance()->SetCommandTick();
			// int vecIndex;

			while(true)
			{
				MCommand command = GetCommandSafe();

				if (command == null)
				{
					break;
				}

				uint commandID = (uint)command.GetID();

				// TODO: nVecIndex = MGetCheckLoopTimeInstance()->AddCommandTimeGap(pCommand->GetID());
				OnPrepareCommand(command);

				if (command.CommandDesc.IsFlag(MCommandDescFlags.MCDT_PEER2PEER))
				{
					if (command.Sender != This)
					{
						OnCommand(command);
					}
					else
					{
						SendCommand(command);
						OnCommand(command);
					}
				}
				else if (command.CommandDesc.IsFlag(MCommandDescFlags.MCDT_LOCAL)
					|| (This.IsValid() && command.Receiver == This))
				{
					OnCommand(command);
				}
				else
				{
					SendCommand(command);
				}

				// TODO: MGetCheckLoopTimeInstance()->SetCommandEndTick(nVecIndex);
			}

			OnRun();
		}

		public abstract void Disconnect(MUID uid);

		protected virtual void ReceiveCommand(MCommand command)
		{
			command.Receiver = This;
			CommandManager.Post(command);
		}

		protected abstract void SendCommand(MCommand command);

		protected virtual void OnRegisterCommand(MCommandManager commandManager)
		{

		}

		protected virtual bool OnCommand(MCommand command)
		{
			return false;
		}

		protected virtual void OnPrepareRun()
		{

		}

		protected virtual void OnPrepareCommand(MCommand command)
		{

		}

		protected virtual void OnRun()
		{

		}

		protected int CalcPacketSize(MCommand command)
		{
			return Marshal.SizeOf(typeof(MPacketHeader)) + command.GetSize();
		}
	}
}
