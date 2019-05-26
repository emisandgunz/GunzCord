using GunzCord.Configuration;
using GunzCord.Database;
using GunzCord.DiscordClient;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord
{
	public class GunzCord
	{
		private readonly IDatabaseService _databaseService;
		private readonly IDiscordService _discordService;

		public GunzCord(
			IDatabaseService databaseService,
			IDiscordService discordService,
			IOptions<AppConfiguration> appConfigurationOptions)
		{
			_databaseService = databaseService;
			_discordService = discordService;

			ConfigureLocale(appConfigurationOptions.Value.Locale);
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			await _databaseService.StartAsync(cancellationToken);
			await _discordService.StartAsync(cancellationToken);

		}

		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			await _discordService.StopAsync(cancellationToken);
			await _databaseService.StopAsync(cancellationToken);
		}

		private void ConfigureLocale(string locale)
		{
			if (!string.IsNullOrEmpty(locale))
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(locale);
				Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(locale);
			}
		}
	}
}
