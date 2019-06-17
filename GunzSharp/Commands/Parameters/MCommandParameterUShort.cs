using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterUShort : MCommandParameter
	{
		public ushort Value { get; set; }

		public MCommandParameterUShort() : base(MCommandParameterType.MPT_USHORT)
		{

		}

		public MCommandParameterUShort(ushort value) : base(MCommandParameterType.MPT_USHORT)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterUShort(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(ushort);

			if (valueSize > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(ushort);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToUInt16(data, index);

			return sizeof(ushort);
		}
	}
}
