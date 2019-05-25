using System;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Application
{
	public interface IApplication : IDisposable
	{
		IServiceProvider Services { get; }

		Task StartAsync(CancellationToken token = default);

		Task StopAsync(CancellationToken token = default);
	}
}
