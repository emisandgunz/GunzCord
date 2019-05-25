using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.Configuration
{
	public class GunZConfiguration
	{
		public string DefaultClanEmblem { get; set; } = string.Empty;

		public string EmblemBaseUrl { get; set; } = string.Empty;

		public byte[] UGradeID_Admin { get; set; } = new byte[] { 255 };

		public byte[] UGradeID_Banned { get; set; } = new byte[] { 253 };

		public byte[] UGradeID_Event { get; set; } = new byte[] { 2 };

		public byte[] UGradeID_GameMaster { get; set; } = new byte[] { 254, 252 };

		public byte[] UGradeID_Normal { get; set; } = new byte[] { 0, 3, 4, 5, 6, 7, 8, 9, 10 };
	}
}
