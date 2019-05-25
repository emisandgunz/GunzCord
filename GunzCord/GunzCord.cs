using GunzCord.Database;
using GunzCord.DiscordClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			IDiscordService discordService)
		{
			_databaseService = databaseService;
			_discordService = discordService;
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
	}
}
