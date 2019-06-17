using System;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterVector : MCommandParameter
	{
		public float X { get; set; }

		public float Y { get; set; }

		public float Z { get; set; }

		public MCommandParameterVector() : base(MCommandParameterType.MPT_VECTOR)
		{
			X = Y = Z = 0;
		}

		public MCommandParameterVector(float x, float y, float z) : base(MCommandParameterType.MPT_VECTOR)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterVector(X, Y, Z);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = sizeof(float) * 3;

			if (valueSize > size)
			{
				return 0;
			}

			float[] v = new float[3] { X, Y, Z };
			Buffer.BlockCopy(v, 0, data, 0, valueSize);

			return valueSize;
		}

		public override int GetSize()
		{
			return sizeof(float) * 3;
		}

		public override void GetValue(out object p)
		{
			p = new float[3] { X, Y, Z };
		}

		public override int SetData(byte[] data, int index)
		{
			int valueSize = sizeof(float) * 3;

			float[] v = new float[3];
			Buffer.BlockCopy(data, index, v, 0, valueSize);

			X = v[0];
			Y = v[1];
			Z = v[2];

			return valueSize;
		}
	}
}
