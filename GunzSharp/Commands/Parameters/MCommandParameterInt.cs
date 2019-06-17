using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterInt : MCommandParameter
	{
		public int Value { get; set; }

		public MCommandParameterInt() : base(MCommandParameterType.MPT_INT)
		{
			Value = 0;
		}

		public MCommandParameterInt(int value) : base(MCommandParameterType.MPT_INT)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterInt(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			if (sizeof(int) > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return sizeof(int);
		}

		public override int GetSize()
		{
			return sizeof(int);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToInt32(data, index);

			return sizeof(int);
		}
	}
}
