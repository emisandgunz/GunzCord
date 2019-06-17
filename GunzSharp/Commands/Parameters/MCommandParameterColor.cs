namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterColor : MCommandParameterVector
	{
		public MCommandParameterColor()
		{
			Type = MCommandParameterType.MPT_COLOR;
		}

		public MCommandParameterColor(float x, float y, float z) : base(x, y, z)
		{
			Type = MCommandParameterType.MPT_COLOR;
		}

		public override MCommandParameter Clone()
		{
			return new MCommandParameterColor(X, Y, Z);
		}
	}
}
