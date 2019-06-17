using System;

namespace GunzSharp
{
	public static class MBlobArray
	{
		public static int GetBlobArrayCount(byte[] blob)
		{
			return BitConverter.ToInt32(blob, sizeof(int));
		}

		public static byte[] GetBlobArrayElement(byte[] blob, int index)
		{
			int blobCount = BitConverter.ToInt32(blob, sizeof(int));
			int blobSize = BitConverter.ToInt32(blob, 0);

			if (index < 0 || index >= blobCount)
			{
				return null;
			}

			byte[] result = new byte[blobSize];

			Buffer.BlockCopy(blob, (sizeof(int) * 2) + (blobSize * index), result, 0, blobSize);

			return result;
		}

		public static int GetBlobArrayElementSize(byte[] blob)
		{
			return BitConverter.ToInt32(blob, 0);
		}

		public static int GetBlobArrayInfoSize()
		{
			return sizeof(int) * 2;
		}

		public static int GetBlobArraySize(byte[] blob)
		{
			int blobCount = BitConverter.ToInt32(blob, sizeof(int));
			int blobSize = BitConverter.ToInt32(blob, 0);

			return (blobSize * blobCount) + (sizeof(int) * 2);
		}

		public static byte[] MakeBlobArray(int blobSize, int blobCount)
		{
			int size = (sizeof(int) * 2) + (blobSize * blobCount);
			byte[] blob = new byte[size];

			BitConverter.GetBytes(blobSize).CopyTo(blob, 0);
			BitConverter.GetBytes(blobCount).CopyTo(blob, sizeof(int));

			return blob;
		}

		public static bool ValidateBlobArraySize(byte[] blob, int size)
		{
			if (size < 8)
			{
				return false;
			}

			int blobSize = GetBlobArrayElementSize(blob);
			int blobCount = GetBlobArrayCount(blob);

			return size == GetBlobArrayInfoSize() + (blobSize * blobCount);
		}
	}
}
