using System;

namespace GunzSharp.Commands
{
	[Flags]
	public enum MSharedCommandType : uint
	{
		Master = 1,
		Client = 2,
		MatchServer = 4,
		Agent = 8,
		All = uint.MaxValue
	}
}
