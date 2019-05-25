using GunzCord.Application;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GunzCord.Startup
{
	class Program
	{
		static void Main(string[] args)
		{
			Startup startup = new Startup();

			IServiceCollection services = new ServiceCollection();
			startup.ConfigureServices(services);

			IServiceProvider serviceProvider = services.BuildServiceProvider();

			serviceProvider.GetService<IGunzCordApplication>().Run();
		}
	}
}
