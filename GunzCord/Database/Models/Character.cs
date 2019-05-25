using System;

namespace GunzCord.Database.Models
{
	public class Character
	{
		public int AID { get; set; }

		public int CID { get; set; }

		public string ClanName { get; set; }

		public int? DeathCount { get; set; }

		public int? KillCount { get; set; }

		public DateTime? LastTime { get; set; }

		public short Level { get; set; }

		public string Name { get; set; }

		public int? PlayTime { get; set; }

		public DateTime? RegDate { get; set; }

		public byte Sex { get; set; }

		public int UGradeID { get; set; }

		public int XP { get; set; }
	}

	public static class CharacterExtensions
	{
		public static string GetKillDeathRatio(this Character character)
		{
			float ratio = 0f;

			if (character.KillCount.HasValue && character.DeathCount.HasValue && character.DeathCount > 0)
			{
				ratio = (float)Math.Round((float)character.KillCount.Value / (float)character.DeathCount.Value, 2);
			}

			return ratio.ToString();
		}

		public static string GetPlayTimeForDisplay(this Character character)
		{
			TimeSpan playTime = TimeSpan.FromSeconds(character.PlayTime.HasValue ? character.PlayTime.Value : 0);

			if (playTime.TotalHours >= 1)
			{
				return $"{ Math.Round(playTime.TotalHours, 0) } { Strings.HOURS }";
			}
			else
			{
				return $"{ Math.Round(playTime.TotalMinutes, 0) } { Strings.MINUTES }";
			}
		}

		public static string GetSexDisplayName(this Character character)
		{
			string sexDisplayName = Strings.UNKNOWN;

			if (character.Sex == 0)
			{
				sexDisplayName = Strings.SEX_MALE;
			}
			else if (character.Sex == 1)
			{
				sexDisplayName = Strings.SEX_FEMALE;
			}

			return sexDisplayName;
		}
	}
}
