namespace GunzSharp.Commands.Parameters.Conditions
{
	public class MCommandParamConditionBlobSize : MCommandParamCondition
	{
		private int size;

		public MCommandParamConditionBlobSize(int size)
		{
			this.size = size;
		}

		public override bool Check(MCommandParameter commandParameter)
		{
			if (commandParameter.Type == MCommandParameterType.MPT_BLOB)
			{
				MCommandParameterBlob blobParameter = (MCommandParameterBlob)commandParameter;

				return blobParameter.GetPayloadSize() == size;
			}

			return false;
		}
	}
}
