using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Database.SQLite
{
	public class SQLiteClanWarNotificationService : IClanWarNotificationService
	{
		public event ClanWarNotificationEventHandlerAsync OnClanWarNotification;

		public Task StartAsync(CancellationToken token = default)
		{
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken token = default)
		{
			return Task.CompletedTask;
		}

		public void Dispose()
		{
		}
	}
}
