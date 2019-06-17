using System;

namespace GunzSharp
{
	public struct MUID
	{
		public static readonly MUID Empty = new MUID(0, 0);

		public uint High { get; set; }

		public uint Low { get; set; }

		public MUID(uint high, uint low)
		{
			High = high;
			Low = low;
		}

		public MUID(ulong value)
		{
			High = (uint)(value >> 32);
			Low = (uint)(value & 0xFFFF_FFFF);
		}

		public void SetZero()
		{
			High = 0;
			Low = 0;
		}

		public void SetInvalid()
		{
			SetZero();
		}

		public MUID Increase(uint size = 1)
		{
			try
			{
				Low = checked(Low + size);
			}
			catch (OverflowException)
			{
				Low = checked(size - (uint.MaxValue - Low));
				High = checked(High + 1);
			}

			return this;
		}

		public bool IsInvalid()
		{
			return High == 0 && Low == 0;
		}

		public bool IsValid()
		{
			return !IsInvalid();
		}

		public static bool operator >(MUID a, MUID b)
		{
			if (a.High > b.High)
			{
				return true;
			}

			if (a.High == b.High && a.Low > b.Low)
			{
				return true;
			}

			return false;
		}

		public static bool operator >=(MUID a, MUID b)
		{
			if (a.High > b.High)
			{
				return true;
			}

			if (a.High == b.High && a.Low >= b.Low)
			{
				return true;
			}

			return false;
		}

		public static bool operator <(MUID a, MUID b)
		{
			if (a.High < b.High)
			{
				return true;
			}

			if (a.High == b.High && a.Low < b.Low)
			{
				return true;
			}

			return false;
		}

		public static bool operator <=(MUID a, MUID b)
		{
			if (a.High < b.High)
			{
				return true;
			}

			if (a.High == b.High && a.Low <= b.Low)
			{
				return true;
			}

			return false;
		}

		public static bool operator ==(MUID a, MUID b)
		{
			return a.High == b.High && a.Low == b.Low;
		}

		public static bool operator !=(MUID a, MUID b)
		{
			return a.High != b.High || a.Low != b.Low;
		}

		public static MUID operator ++(MUID v)
		{
			return v.Increase();
		}

		public long AsLong()
		{
			return ((long)High) << 32 | Low;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MUID))
			{
				return false;
			}

			return High == ((MUID)obj).High && Low == ((MUID)obj).Low;
		}

		public override int GetHashCode()
		{
			return new { High, Low }.GetHashCode();
		}
	}

	public struct MUIDRANGE
	{
		public MUID Start { get; set; }

		public MUID End { get; set; }

		public bool IsEmpty()
		{
			return Start == End;
		}

		public void Empty()
		{
			SetZero();
		}

		public void SetZero()
		{
			Start.SetZero();
			End.SetZero();
		}
	}
}
