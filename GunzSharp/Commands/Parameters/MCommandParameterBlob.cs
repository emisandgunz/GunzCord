using System;
using System.Linq;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterBlob : MCommandParameter
	{
		public const int MAX_BLOB_SIZE = 0x100000;

		public byte[] Value { get; set; }

		public MCommandParameterBlob() : base(MCommandParameterType.MPT_BLOB)
		{

		}

		public MCommandParameterBlob(uint size) : base(MCommandParameterType.MPT_BLOB)
		{
			if (size > MAX_BLOB_SIZE)
			{
				Value = null;
				return;
			}

			Value = new byte[size];
		}

		public MCommandParameterBlob(byte[] value) : base(MCommandParameterType.MPT_BLOB)
		{
			if (value.Length > MAX_BLOB_SIZE)
			{
				Value = null;
				return;
			}

			Value = value.ToArray();
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterBlob(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			if (Value == null)
			{
				return 0;
			}

			if (Value.Length + sizeof(int) > size)
			{
				return 0;
			}

			BitConverter.GetBytes(Value.Length).CopyTo(data, 0);
			Value.CopyTo(data, sizeof(int));

			return Value.Length + sizeof(int);
		}

		public int GetPayloadSize()
		{
			return Value.Length;
		}

		public override int GetSize()
		{
			return Value.Length + sizeof(int);
		}

		public override void GetValue(out object p)
		{
			p = Value.ToArray();
		}

		public override int SetData(byte[] data, int index)
		{
			int size = BitConverter.ToInt32(data, index);

			if (size > MAX_BLOB_SIZE)
			{
				Value = null;
				return sizeof(int);
			}

			Value = new byte[size];
			Buffer.BlockCopy(data, index + sizeof(int), Value, 0, size);

			return size + sizeof(int);
		}
	}
}
