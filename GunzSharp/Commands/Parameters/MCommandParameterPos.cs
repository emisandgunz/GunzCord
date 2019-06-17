namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterPos : MCommandParameterVector
	{
		public MCommandParameterPos()
		{
			Type = MCommandParameterType.MPT_POS;
		}

		public MCommandParameterPos(float x, float y, float z) : base(x, y, z)
		{
			Type = MCommandParameterType.MPT_POS;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterPos(X, Y, Z);
		}
	}
}
