using GunzCord.Database.Events;
using GunzCord.Database.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace GunzCord.Database
{
	public class ClanWarNotificationService : IClanWarNotificationService
	{
		private readonly IDatabaseService _databaseService;
		private readonly ILogger<ClanWarNotificationService> _logger;
		private SqlTableDependency<ClanGameLog> _clanGameLogDependency;

		public event ClanWarNotificationEventHandlerAsync OnClanWarNotification;

		public ClanWarNotificationService(IDatabaseService databaseService, ILogger<ClanWarNotificationService> logger)
		{
			_databaseService = databaseService;
			_logger = logger;
		}

		public Task StartAsync(CancellationToken token = default)
		{
			if (_clanGameLogDependency == null)
			{
				_logger.LogInformation("Starting the Clan War Notification service");

				var mapper = new ModelToTableMapper<ClanGameLog>();
				mapper.AddMapping(x => x.Id, "id");

				_clanGameLogDependency = new SqlTableDependency<ClanGameLog>(_databaseService.ConnectionString, "ClanGameLog", "dbo", mapper: mapper, notifyOn: DmlTriggerType.Insert, executeUserPermissionCheck: true);
				_clanGameLogDependency.OnChanged += OnClanGameLogChanged;

				_clanGameLogDependency.Start();
			}

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken token = default)
		{
			if (_clanGameLogDependency != null)
			{
				_logger.LogInformation("Stopping the Clan War Notification service");

				_clanGameLogDependency.Stop();
				_clanGameLogDependency.Dispose();
				_clanGameLogDependency = null;
			}

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			if (_clanGameLogDependency != null)
			{
				_clanGameLogDependency.Dispose();
			}
		}

		private void OnClanGameLogChanged(object sender, RecordChangedEventArgs<ClanGameLog> e)
		{
			if (e.ChangeType == ChangeType.Insert)
			{
				_logger.LogInformation("Clan \"{0}\" won to clan \"{1}\"", e.Entity.WinnerClanName, e.Entity.LoserClanName);

				ClanWarNotificationEventHandlerAsync handler = OnClanWarNotification;
				handler?.Invoke(this, new ClanWarNotificationEventArgs(e.Entity));
			}
		}
	}
}
