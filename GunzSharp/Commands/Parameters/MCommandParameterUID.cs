using System;
using System.Runtime.InteropServices;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterUID : MCommandParameter
	{
		[MarshalAs(UnmanagedType.Struct)]
		public MUID Value;

		public MCommandParameterUID() : base(MCommandParameterType.MPT_UID)
		{

		}

		public MCommandParameterUID(MUID value) : base(MCommandParameterType.MPT_UID)
		{
			Value = value;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterUID(Value);
		}

		public override int GetData(ref byte[] data, int size)
		{
			int valueSize = Marshal.SizeOf(typeof(MUID));

			if (valueSize > size)
			{
				return 0;
			}

			IntPtr ptr = Marshal.AllocHGlobal(valueSize);
			Marshal.StructureToPtr(Value, ptr, true);
			Marshal.Copy(ptr, data, 0, valueSize);
			Marshal.FreeHGlobal(ptr);

			return valueSize;
		}

		public override int GetSize()
		{
			return Marshal.SizeOf(typeof(MUID));
		}

		public override void GetValue(out object p)
		{
			p = Value;
		}

		public override int SetData(byte[] data, int index)
		{
			int valueSize = Marshal.SizeOf(typeof(MUID));

			IntPtr ptr = Marshal.AllocHGlobal(valueSize);
			Marshal.Copy(data, 0, ptr, valueSize);
			Marshal.PtrToStructure<MUID>(ptr, Value);
			Marshal.FreeHGlobal(ptr);

			return valueSize;
		}
	}
}
