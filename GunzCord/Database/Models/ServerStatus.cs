using System;

namespace GunzCord.Database.Models
{
	public class ServerStatus
	{
		public int ServerID { get; set; }

		public string ServerName { get; set; }

		public int CurrPlayer { get; set; }

		public int MaxPlayer { get; set; }

		public DateTime? Time { get; set; }
	}
}
