using GunzCord.Database;

namespace GunzCord.Configuration
{
	public class AppConfiguration
	{
		public string DatabaseType { get; set; } = DatabaseTypes.MICROSOFT_SQL_SERVER;

		public string Locale { get; set; } = "en-US";
	}
}
