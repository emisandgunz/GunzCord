using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterUInt64 : MCommandParameter
	{
		public ulong Value { get; set; }

		public MCommandParameterUInt64() : base(MCommandParameterType.MPT_UINT64)
		{

		}

		public MCommandParameterUInt64(ulong value) : base(MCommandParameterType.MPT_UINT64)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterUInt64(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(ulong);

			if (valueSize > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(ulong);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToUInt64(data, index);

			return sizeof(ulong);
		}
	}
}
