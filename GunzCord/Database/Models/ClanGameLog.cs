using System;

namespace GunzCord.Database.Models
{
	public class ClanGameLog
	{
		public byte GameType { get; set; }

		public int Id { get; set; }

		public string LoserClanName { get; set; }

		public int LoserCLID { get; set; }

		public string LoserMembers { get; set; }

		public int LoserPoint { get; set; }

		public byte MapID { get; set; }

		public DateTime RegDate { get; set; }

		public byte RoundLosses { get; set; }

		public byte RoundWins { get; set; }

		public string WinnerClanName { get; set; }

		public int WinnerCLID { get; set; }

		public string WinnerMembers { get; set; }

		public int WinnerPoint { get; set; }
	}
}
