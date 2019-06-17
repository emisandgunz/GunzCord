using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterFloat : MCommandParameter
	{
		public float Value { get; set; }

		public MCommandParameterFloat() : base(MCommandParameterType.MPT_FLOAT)
		{
			Value = 0f;
		}

		public MCommandParameterFloat(float value) : base(MCommandParameterType.MPT_FLOAT)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterFloat(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			if (sizeof(float) > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return sizeof(float);
		}

		public override int GetSize()
		{
			return sizeof(float);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToSingle(data, index);

			return sizeof(float);
		}
	}
}
