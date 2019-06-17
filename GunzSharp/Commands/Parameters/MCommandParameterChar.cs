using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterChar : MCommandParameter
	{
		public char Value { get; set; }

		public MCommandParameterChar() : base(MCommandParameterType.MPT_CHAR)
		{

		}

		public MCommandParameterChar(char value) : base(MCommandParameterType.MPT_CHAR)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterChar(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(char);

			if (valueSize > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(char);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToChar(data, index);

			return sizeof(char);
		}
	}
}
