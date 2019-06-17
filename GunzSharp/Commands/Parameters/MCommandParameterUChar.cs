using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterUChar : MCommandParameter
	{
		public byte Value { get; set; }

		public MCommandParameterUChar() : base(MCommandParameterType.MPT_UCHAR)
		{

		}

		public MCommandParameterUChar(byte value) : base(MCommandParameterType.MPT_UCHAR)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterUChar(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(byte);

			if (valueSize > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(byte);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = data[index];

			return sizeof(byte);
		}
	}
}
