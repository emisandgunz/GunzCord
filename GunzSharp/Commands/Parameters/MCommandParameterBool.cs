using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterBool : MCommandParameter
	{
		public bool Value { get; set; }


		public MCommandParameterBool() : base(MCommandParameterType.MPT_BOOL)
		{
		}

		public MCommandParameterBool(bool value) : base(MCommandParameterType.MPT_BOOL)
		{
			Value = value;
		}


		public override MCommandParameter Clone()
		{
			return new MCommandParameterBool(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			if (sizeof(bool) > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value).CopyTo(data, 0);

			return sizeof(bool);
		}

		public override int GetSize()
		{
			return sizeof(bool);
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			Value = BitConverter.ToBoolean(data, index);

			return sizeof(bool);
		}
	}
}
