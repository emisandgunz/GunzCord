using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterUInt : MCommandParameter
	{
		public uint Value { get; set; }

		public MCommandParameterUInt() : base(MCommandParameterType.MPT_UINT)
		{
			Value = 0;
		}

		public MCommandParameterUInt(uint value) : base(MCommandParameterType.MPT_UINT)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterUInt(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			if (sizeof(uint) > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return sizeof(uint);
		}

		public override int GetSize()
		{
			return sizeof(uint);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToUInt32(data, index);

			return sizeof(uint);
		}
	}
}
