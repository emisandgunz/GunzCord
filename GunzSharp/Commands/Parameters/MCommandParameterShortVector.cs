using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterShortVector : MCommandParameter
	{
		public short X { get; set; }

		public short Y { get; set; }

		public short Z { get; set; }

		public MCommandParameterShortVector() : base(MCommandParameterType.MPT_SVECTOR)
		{

		}

		public MCommandParameterShortVector(short x, short y, short z) : base(MCommandParameterType.MPT_SVECTOR)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public MCommandParameterShortVector(float x, float y, float z) : base(MCommandParameterType.MPT_SVECTOR)
		{
			X = (short)Math.Floor(x + 0.5f);
			Y = (short)Math.Floor(y + 0.5f);
			Z = (short)Math.Floor(z + 0.5f);
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterShortVector(X, Y, Z);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(short) * 3;

			if (valueSize > size)
			{
				return 0;
			}

			short[] v = new short[3] { X, Y, Z };

			Buffer.BlockCopy(v, 0, data, 0, valueSize);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(short) * 3;
		}

		public override void GetValue(out object p)
		{
			p = new float[3] { X, Y, Z };
		}

		public override int SetData(byte[] data, int index)
		{
			int valueSize = sizeof(short) * 3;

			short[] v = new short[3];
			Buffer.BlockCopy(data, index, v, 0, valueSize);

			X = v[0];
			Y = v[1];
			Z = v[2];

			return valueSize;
		}
	}
}
