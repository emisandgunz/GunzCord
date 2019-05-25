using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.Database.Models
{
	public class Clan
	{
		public int CLID { get; set; }

		public int Draws { get; set; }

		public string EmblemUrl { get; set; }

		public string Leader { get; set; }

		public int Losses { get; set; }

		public int MasterCID { get; set; }

		public string Name { get; set; }

		public int Point { get; set; }

		public int Ranking { get; set; }

		public int Wins { get; set; }
	}
}
