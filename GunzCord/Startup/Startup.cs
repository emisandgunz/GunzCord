using GunzCord.Application;
using GunzCord.Configuration;
using GunzCord.Database;
using GunzCord.DiscordClient;
using GunzCord.DiscordClient.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace GunzCord.Startup
{
	public class Startup
	{
		IConfigurationRoot Configuration { get; }

		public Startup()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<AppConfiguration>(Configuration.GetSection("App"));
			services.Configure<DiscordConfiguration>(Configuration.GetSection("Discord"));
			services.Configure<GunZConfiguration>(Configuration.GetSection("GunZ"));

			services.AddLogging(configure =>
			{
				configure.AddLog4Net("log4net.config", true);
				configure.SetMinimumLevel(LogLevel.Debug);
			});

			services.AddSingleton(Configuration);
			
			services.AddSingleton<IDatabaseService, SqlDatabaseService>();
			services.AddTransient<IDiscordService, DiscordService>();
			services.AddTransient<IGunzRepository, GunzRepository>();

			services.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
			services.AddTransient<IGunzCordApplication, GunzCordApplication>();
			services.AddTransient<GunzCord>();

			services.AddTransient<GunzModule>();
		}
	}
}
