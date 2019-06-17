namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterDir : MCommandParameterVector
	{
		public MCommandParameterDir()
		{
			Type = MCommandParameterType.MPT_DIR;
		}

		public MCommandParameterDir(float x, float y, float z) : base(x, y, z)
		{
			Type = MCommandParameterType.MPT_DIR;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterDir(X, Y, Z);
		}
	}
}
