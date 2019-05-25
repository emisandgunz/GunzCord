using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Application
{
	public interface IApplicationService
	{
		Task StartAsync(CancellationToken token = default);

		Task StopAsync(CancellationToken token = default);
	}
}
