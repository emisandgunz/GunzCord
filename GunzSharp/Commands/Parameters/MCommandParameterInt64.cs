using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterInt64 : MCommandParameter
	{
		public long Value { get; set; }

		public MCommandParameterInt64() : base(MCommandParameterType.MPT_INT64)
		{

		}

		public MCommandParameterInt64(long value) : base(MCommandParameterType.MPT_INT64)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterInt64(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(long);

			if (valueSize > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(long);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToInt64(data, index);

			return sizeof(long);
		}
	}
}
