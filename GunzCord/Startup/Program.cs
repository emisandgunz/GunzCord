using GunzCord.Application;
using GunzCord.Configuration;
using GunzCord.Database;
using GunzCord.Database.SQLite;
using GunzCord.Database.SqlServer;
using GunzCord.DiscordClient;
using GunzCord.DiscordClient.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace GunzCord.Startup
{
	class Program
	{
		static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			.UseConsoleLifetime()
			.ConfigureLogging((logging) =>
			{
				logging.ClearProviders();

				logging.AddLog4Net("log4net.config", true);
				logging.SetMinimumLevel(LogLevel.Information);
			})
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				configurationBuilder
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			})
			.ConfigureServices((hostBuilderContext, services) =>
			{
				var configuration = hostBuilderContext.Configuration;

				services.Configure<AppConfiguration>(configuration.GetSection("App"));
				services.Configure<DiscordConfiguration>(configuration.GetSection("Discord"));
				services.Configure<GunZConfiguration>(configuration.GetSection("GunZ"));

				string databaseType = configuration["App:DatabaseType"];

				if (!string.IsNullOrEmpty(databaseType) && databaseType.Equals(DatabaseTypes.SQLITE3, StringComparison.OrdinalIgnoreCase))
				{
					services.AddSingleton<IDatabaseService, SQLiteDatabaseService>();
					services.AddSingleton<IClanWarNotificationService, SQLiteClanWarNotificationService>();

					services.AddTransient<IGunzRepository, SQLiteGunzRepository>();
				}
				else
				{
					services.AddSingleton<IDatabaseService, SqlDatabaseService>();
					services.AddSingleton<IClanWarNotificationService, SqlClanWarNotificationService>();

					services.AddTransient<IGunzRepository, SqlGunzRepository>();
				}

				services.AddSingleton<IDiscordService, DiscordService>();

				services.AddTransient<GunzCord>();
				services.AddTransient<GunzModule>();

				services.AddHostedService<GunzCordApplication>();
			});
	}
}
