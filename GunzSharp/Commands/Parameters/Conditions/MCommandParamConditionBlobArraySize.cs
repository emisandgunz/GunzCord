namespace GunzSharp.Commands.Parameters.Conditions
{
	public class MCommandParamConditionBlobArraySize : MCommandParamCondition
	{
		private int size;
		private int minCount;
		private int maxCount;

		public MCommandParamConditionBlobArraySize(int size, int minCount = -1, int maxCount = -1)
		{
			this.size = size;
			this.minCount = minCount;
			this.maxCount = maxCount;
		}

		public override bool Check(MCommandParameter commandParameter)
		{
			if (commandParameter.Type == MCommandParameterType.MPT_BLOB)
			{
				MCommandParameterBlob blobParameter = (MCommandParameterBlob)commandParameter;

				int blobSize = blobParameter.GetPayloadSize();

				if (!MBlobArray.ValidateBlobArraySize(blobParameter.Value, blobSize))
				{
					return false;
				}

				int blobElementSize = MBlobArray.GetBlobArrayElementSize(blobParameter.Value);

				if (blobElementSize != size)
				{
					return false;
				}

				int blobCount = MBlobArray.GetBlobArrayCount(blobParameter.Value);

				if (minCount != -1 && blobCount < minCount)
				{
					return false;
				}

				if (maxCount != -1 && blobCount > maxCount)
				{
					return false;
				}

				return true;
			}

			return false;
		}
	}
}
