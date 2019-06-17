using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterShort : MCommandParameter
	{
		public short Value { get; set; }

		public MCommandParameterShort() : base(MCommandParameterType.MPT_SHORT)
		{

		}

		public MCommandParameterShort(short value) : base(MCommandParameterType.MPT_SHORT)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterShort(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(short);

			if (valueSize > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(short);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToInt16(data, index);

			return sizeof(short);
		}
	}
}
