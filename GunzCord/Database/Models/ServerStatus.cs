using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.Database.Models
{
	public class ServerStatus
	{
		public int ServerID { get; set; }

		public string Name { get; set; }

		public int CurPlayer { get; set; }

		public int MaxPlayer { get; set; }
	}
}
