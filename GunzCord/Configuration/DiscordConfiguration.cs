namespace GunzCord.Configuration
{
	public class DiscordConfiguration
	{
		public string CommandPrefix { get; set; } = "!";

		public bool EnableLogging { get; set; } = false;

		public string FooterSignature { get; set; } = "GunzCord";

		public string Token { get; set; } = string.Empty;
	}
}
