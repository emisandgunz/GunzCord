namespace GunzSharp.Commands.Parameters
{
	public abstract class MCommandParameter
	{
		public MCommandParameterType Type { get; protected set; }

		public MCommandParameter(MCommandParameterType type)
		{
			Type = type;
		}

		public abstract MCommandParameter Clone();

		public abstract int GetData(ref byte[] data, int size);

		public abstract int GetSize();

		public abstract void GetValue(out object p);

		public abstract int SetData(byte[] data, int index);
	}
}
