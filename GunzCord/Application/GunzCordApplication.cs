using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Application
{
	public class GunzCordApplication : BackgroundService
	{
		private readonly GunzCord _gunzCord;

		private bool _stopped;

		public GunzCordApplication(GunzCord gunzCord)
		{
			_gunzCord = gunzCord;
		}

		public override void Dispose()
		{
			if (!_stopped)
			{
				try
				{
					StopAsync().GetAwaiter().GetResult();
				}
				catch { }
			}
		}

		public override async Task StopAsync(CancellationToken cancellationToken = default)
		{
			if (_stopped)
			{
				return;
			}

			_stopped = true;

			var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
			if (!cancellationToken.CanBeCanceled)
			{
				cancellationToken = timeoutToken;
			}
			else
			{
				cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken).Token;
			}


			await _gunzCord.StopAsync(cancellationToken).ConfigureAwait(false);
			await base.StopAsync(cancellationToken);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _gunzCord.StartAsync(stoppingToken).ConfigureAwait(false);
		}
	}
}
