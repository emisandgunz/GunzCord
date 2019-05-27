using GunzCord.Database.Models;
using System;

namespace GunzCord.Database.Events
{
	public class ClanWarNotificationEventArgs : EventArgs
	{
		public ClanGameLog ClanGameLog { get; set; }

		public ClanWarNotificationEventArgs()
		{

		}

		public ClanWarNotificationEventArgs(ClanGameLog clanGameLog)
		{
			ClanGameLog = clanGameLog;
		}
	}
}
