namespace GunzSharp.Commands.Parameters.Conditions
{
	public class MCommandParamConditionMinMax : MCommandParamCondition
	{
		private int min;
		private int max;

		public MCommandParamConditionMinMax(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public override bool Check(MCommandParameter commandParameter)
		{
			object value;

			switch (commandParameter.Type)
			{
				case MCommandParameterType.MPT_INT:
					commandParameter.GetValue(out value);

					if ((int)value < min || (int)value > max)
					{
						return false;
					}
					break;
				case MCommandParameterType.MPT_UINT:
					commandParameter.GetValue(out value);

					if ((uint)value < (uint)min || (uint)value > (uint)max)
					{
						return false;
					}
					break;
				case MCommandParameterType.MPT_CHAR:
					commandParameter.GetValue(out value);

					if ((char)value < (char)min || (char)value > (char)max)
					{
						return false;
					}
					break;
				case MCommandParameterType.MPT_UCHAR:
					commandParameter.GetValue(out value);

					if ((byte)value < (byte)min || (byte)value > (byte)max)
					{
						return false;
					}
					break;
				case MCommandParameterType.MPT_SHORT:
					commandParameter.GetValue(out value);

					if ((short)value < (short)min || (short)value > (short)max)
					{
						return false;
					}
					break;
				case MCommandParameterType.MPT_USHORT:
					commandParameter.GetValue(out value);

					if ((ushort)value < (ushort)min || (ushort)value > (ushort)max)
					{
						return false;
					}
					break;
			}

			return true;
		}
	}
}
