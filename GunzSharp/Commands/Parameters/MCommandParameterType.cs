namespace GunzSharp.Commands.Parameters
{
	public enum MCommandParameterType
	{
		MPT_INT = 0,
		MPT_UINT = 1,
		MPT_FLOAT = 2,
		MPT_BOOL = 3,
		MPT_STR = 4,
		MPT_VECTOR = 5,
		MPT_POS = 6,
		MPT_DIR = 7,
		MPT_COLOR = 8,
		MPT_UID = 9,
		MPT_BLOB = 10,

		MPT_CHAR = 11,
		MPT_UCHAR = 12,
		MPT_SHORT = 13,
		MPT_USHORT = 14,
		MPT_INT64 = 15,
		MPT_UINT64 = 16,

		MPT_SVECTOR = 17,
		MPT_CMD = 18,
		MPT_END = 19,
	};
}
