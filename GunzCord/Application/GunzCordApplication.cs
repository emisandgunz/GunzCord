using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Application
{
	public class GunzCordApplication : IGunzCordApplication
	{
		private readonly GunzCord _gunzCord;

		private ApplicationLifetime _applicationLifetime;
		private bool _stopped;

		public IServiceProvider Services { get; }

		public GunzCordApplication(IServiceProvider services)
		{
			Services = services;

			_gunzCord = services.GetService<GunzCord>();
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			_applicationLifetime = Services.GetRequiredService<IApplicationLifetime>() as ApplicationLifetime;

			await _gunzCord.StartAsync(cancellationToken).ConfigureAwait(false);

			_applicationLifetime?.NotifyStarted();
		}

		public async Task StopAsync(CancellationToken cancellationToken = default)
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

			_applicationLifetime?.StopApplication();

			await _gunzCord.StopAsync(cancellationToken).ConfigureAwait(false);

			_applicationLifetime?.NotifyStopped();
		}

		public void Dispose()
		{
			if (!_stopped)
			{
				try
				{
					StopAsync().GetAwaiter().GetResult();
				}
				catch { }
			}

			(Services as IDisposable)?.Dispose();
		}
	}
}
