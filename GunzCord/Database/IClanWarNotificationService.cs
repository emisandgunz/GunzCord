using GunzCord.Application;
using GunzCord.Database.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.Database
{
	public delegate Task ClanWarNotificationEventHandlerAsync(object sender, ClanWarNotificationEventArgs e);

	public interface IClanWarNotificationService : IApplicationService, IDisposable
	{
		event ClanWarNotificationEventHandlerAsync OnClanWarNotification;
	}
}
