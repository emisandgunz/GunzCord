using GunzSharp.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunzSharp.Commands
{
	public class MCommand
	{
		public MCommandDesc CommandDesc { get; set; }

		public List<MCommandParameter> Params { get; set; }

		public MUID Receiver { get; set; }

		public MUID Sender { get; set; }

		public byte SerialNumber { get; set; }

		public MCommand()
		{
			Reset();
		}

		public MCommand(MCommandDesc commandDesc, MUID receiver, MUID sender)
		{
			Reset();
			SetID(commandDesc);
			Sender = sender;
			Receiver = receiver;
		}

		public MCommand(int id, MUID sender, MUID receiver, MCommandManager commandManager)
		{
			Reset();
			SetID(id, commandManager);
			Sender = sender;
			Receiver = receiver;
		}

		protected void ClearParam()
		{
			Params = new List<MCommandParameter>();
		}

		public bool AddParameter(MCommandParameter param)
		{
			int count = Params.Count;
			int paramDescCount = CommandDesc.GetParameterDescCount();

			if (count >= paramDescCount)
			{
				return false;
			}

			MCommandParameterDesc paramDesc = CommandDesc.GetParameterDesc(count);

			if (param.Type != paramDesc.Type)
			{
				return false;
			}

			Params.Add(param);

			return true;
		}

		public void ClearParam(int i)
		{
			if (Params.Count > i)
			{
				Params.RemoveAt(i);
			}
		}

		public MCommand Clone()
		{
			if (CommandDesc == null)
			{
				return null;
			}

			MCommand clone = new MCommand(CommandDesc, Receiver, Sender);

			foreach (var param in Params)
			{
				if (!clone.AddParameter(param.Clone()))
				{
					return null;
				}
			}

			return clone;
		}

		public bool CheckRule()
		{
			if (CommandDesc == null)
			{
				return false;
			}

			if (GetParameterCount() != CommandDesc.GetParameterDescCount())
			{
				return false;
			}

			for (int i = 0; i < GetParameterCount(); i++)
			{
				MCommandParameter param = GetParameter(i);
				MCommandParameterDesc paramDesc = CommandDesc.GetParameterDesc(i);

				if (param.Type != paramDesc.Type)
				{
					return false;
				}

				if (paramDesc.HasConditions())
				{
					for (int j = 0; j < paramDesc.GetConditionCount(); j++)
					{
						MCommandParamCondition condition = paramDesc.GetCondition(j);

						if (!condition.Check(param))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public int GetData(ref byte[] data, int size)
		{
			if (CommandDesc == null)
			{
				return 0;
			}

			int paramCount = GetParameterCount();
			ushort dataCount = sizeof(ushort);

			ushort commandId = (ushort)CommandDesc.ID;
			BitConverter.GetBytes(commandId).CopyTo(data, dataCount);
			dataCount += sizeof(ushort);

			BitConverter.GetBytes(SerialNumber).CopyTo(data, dataCount);
			dataCount += sizeof(byte);

			for (int i = 0; i < paramCount; i++)
			{
				MCommandParameter param = GetParameter(i);

				byte[] paramData = new byte[param.GetSize()];
				param.GetData(ref paramData, size - dataCount);

				paramData.CopyTo(data, dataCount);
				dataCount += (ushort)paramData.Length;
			}

			BitConverter.GetBytes(dataCount).CopyTo(data, 0);

			return dataCount;
		}

		public string GetDescription()
		{
			return CommandDesc.Description;
		}

		public int GetID()
		{
			return CommandDesc.ID;
		}

		public int GetParameterCount()
		{
			return Params.Count;
		}

		public MCommandParameter GetParameter(int i)
		{
			if (Params == null || i < 0 || i >= Params.Count)
			{
				return null;
			}

			return Params[i];
		}

		public bool GetParameter(out object value, int i, MCommandParameterType t, int bufferSize)
		{
			MCommandParameter param = GetParameter(i);

			if (param == null)
			{
				value = null;
				return false;
			}

			if (param.Type != t)
			{
				value = null;
				return false;
			}

			if (param.Type == MCommandParameterType.MPT_STR && bufferSize >= 0)
			{
				object paramValue;
				param.GetValue(out paramValue);
				string paramString = (string)paramValue;

				if (paramString != null && paramString.Length >= bufferSize - 1)
				{
					value = paramString.Substring(0, bufferSize - 1);
				}
				else
				{
					value = paramString;
				}
			}
			else
			{
				param.GetValue(out value);
			}

			return true;
		}

		public int GetSize()
		{
			if (CommandDesc == null)
			{
				return 0;
			}

			int size = sizeof(uint) + sizeof(uint) + sizeof(byte);

			for (int i = 0; i < Params.Count; i++)
			{
				size += Params[i].GetSize();
			}

			return size;
		}

		public bool IsLocalCommand()
		{
			return Sender == Receiver;
		}

		public void Reset()
		{
			CommandDesc = null;
			SerialNumber = 0;
			Sender.SetZero();
			Receiver.SetZero();
			ClearParam();
		}

		public bool SetData(byte[] data, MCommandManager cm, ushort dataLen)
		{
			Reset();

			ushort dataCount = 0;

			ushort totalSize = BitConverter.ToUInt16(data, 0);

			if (dataLen != ushort.MaxValue && dataLen != totalSize)
			{
				return false;
			}

			dataCount += sizeof(ushort);

			ushort commandID = BitConverter.ToUInt16(data, dataCount);
			dataCount += sizeof(ushort);

			MCommandDesc desc = cm.GetCommandDescByID(commandID);

			if (desc == null)
			{
				return false;
			}

			SetID(desc);

			SerialNumber = data[dataCount];
			dataCount += sizeof(byte);

			int paramCount = desc.GetParameterDescCount();

			for (int i = 0; i < paramCount; i++)
			{
				MCommandParameterType paramType = desc.GetParameterType(i);

				MCommandParameter param = null;

				switch (paramType)
				{
					case MCommandParameterType.MPT_INT:
						param = new MCommandParameterInt();
						break;
					case MCommandParameterType.MPT_UINT:
						param = new MCommandParameterUInt();
						break;
					case MCommandParameterType.MPT_FLOAT:
						param = new MCommandParameterFloat();
						break;
					case MCommandParameterType.MPT_STR:
						param = new MCommandParameterString();
						break;
					case MCommandParameterType.MPT_VECTOR:
						param = new MCommandParameterVector();
						break;
					case MCommandParameterType.MPT_POS:
						param = new MCommandParameterPos();
						break;
					case MCommandParameterType.MPT_DIR:
						param = new MCommandParameterDir();
						break;
					case MCommandParameterType.MPT_BOOL:
						param = new MCommandParameterBool();
						break;
					case MCommandParameterType.MPT_COLOR:
						param = new MCommandParameterColor();
						break;
					case MCommandParameterType.MPT_UID:
						param = new MCommandParameterUID();
						break;
					case MCommandParameterType.MPT_BLOB:
						param = new MCommandParameterBlob();
						break;
					case MCommandParameterType.MPT_CHAR:
						param = new MCommandParameterChar();
						break;
					case MCommandParameterType.MPT_UCHAR:
						param = new MCommandParameterUChar();
						break;
					case MCommandParameterType.MPT_SHORT:
						param = new MCommandParameterShort();
						break;
					case MCommandParameterType.MPT_USHORT:
						param = new MCommandParameterUShort();
						break;
					case MCommandParameterType.MPT_INT64:
						param = new MCommandParameterInt64();
						break;
					case MCommandParameterType.MPT_UINT64:
						param = new MCommandParameterUInt64();
						break;
					case MCommandParameterType.MPT_SVECTOR:
						param = new MCommandParameterShortVector();
						break;
					default:
						return false;
				}

				dataCount += (ushort)param.SetData(data, dataCount);

				Params.Add(param);

				if (dataCount > totalSize)
				{
					return false;
				}
			}

			if (dataCount != totalSize)
			{
				return false;
			}

			return true;
		}

		public void SetID(MCommandDesc commandDesc)
		{
			CommandDesc = commandDesc;
		}

		public void SetID(int id, MCommandManager commandManager)
		{
			CommandDesc = commandManager.GetCommandDescByID(id);
		}
	}
}
