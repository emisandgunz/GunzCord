using GunzCord.Application;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Database
{
	public interface IDatabaseService : IApplicationService, IDisposable
	{
		IDbConnection Connection { get; }

		string ConnectionString { get; }
	}
}
