using System;
using System.Text;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterString : MCommandParameter
	{
		public string Value { get; set; }

		public MCommandParameterString() : base(MCommandParameterType.MPT_STR)
		{
			Value = null;
		}

		public MCommandParameterString(string value) : base(MCommandParameterType.MPT_STR)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterString(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			if (string.IsNullOrEmpty(Value))
			{
				ushort emptySize = 0;
				BitConverter.GetBytes(emptySize).CopyTo(data, 0);
				return sizeof(ushort);
			}

			ushort valueSize = Convert.ToUInt16(Value.Length + 2);

			if (valueSize + sizeof(ushort) > size)
			{
				return 0;
			}

			BitConverter.GetBytes(valueSize).CopyTo(data, 0);
			Encoding.UTF8.GetBytes(Value).CopyTo(data, sizeof(ushort));

			return valueSize + sizeof(uint);
		}

		public override int GetSize()
		{
			if (string.IsNullOrEmpty(Value))
			{
				return 0;
			}

			return Value.Length + 2 + sizeof(ushort);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = null;

			ushort valueSize = BitConverter.ToUInt16(data, index);

			if (valueSize > ushort.MaxValue - 2 || valueSize == 0)
			{
				return sizeof(uint);
			}

			Value = Encoding.UTF8.GetString(data, index + sizeof(uint), valueSize - 2);

			return valueSize + sizeof(uint);
		}
	}
}
