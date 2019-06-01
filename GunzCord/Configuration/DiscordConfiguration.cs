namespace GunzCord.Configuration
{
	public class DiscordConfiguration
	{
		public string CommandPrefix { get; set; } = "!";

		public bool EnableClanWarNotifications { get; set; } = false;

		public bool EnableLogging { get; set; } = false;

		public string FooterSignature { get; set; } = "GunzCord";

		public ulong? NotificationsChannelId { get; set; }

		public ulong? ServerId { get; set; }

		public string Token { get; set; } = string.Empty;
	}
}
