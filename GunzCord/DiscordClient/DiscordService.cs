using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GunzCord.Configuration;
using GunzCord.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.DiscordClient
{
	public class DiscordService : IDiscordService
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly DiscordConfiguration _discordConfiguration;
		private readonly ILogger<DiscordService> _logger;
		private readonly IServiceProvider _serviceProvider;

		public DiscordService(IOptions<DiscordConfiguration> discordConfigurationOptions, ILogger<DiscordService> logger, IServiceProvider serviceProvider)
		{
			_discordConfiguration = discordConfigurationOptions.Value;
			_logger = logger;
			_serviceProvider = serviceProvider;

			_client = new DiscordSocketClient(new DiscordSocketConfig()
			{
				LogLevel = LogSeverity.Info
			});

			_commands = new CommandService(new CommandServiceConfig()
			{
				CaseSensitiveCommands = false,
				LogLevel = LogSeverity.Info
			});

			if (_discordConfiguration.EnableLogging)
			{
				_client.Log += OnDiscordLog;
				_commands.Log += OnDiscordLog;
			}
		}

		public async Task InstallCommandsAsync()
		{
			// Hook the MessageReceived event into our command handler
			_client.MessageReceived += HandleCommandAsync;

			// Here we discover all of the command modules in the entry 
			// assembly and load them. Starting from Discord.NET 2.0, a
			// service provider is required to be passed into the
			// module registration method to inject the 
			// required dependencies.
			//
			// If you do not use Dependency Injection, pass null.
			// See Dependency Injection guide for more information.
			await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _serviceProvider);
		}

		private async Task HandleCommandAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a system message
			var message = messageParam as SocketUserMessage;
			if (message == null) return;

			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;

			// Determine if the message is a command based on the prefix and make sure no bots trigger commands
			if (!(message.HasStringPrefix(_discordConfiguration.CommandPrefix, ref argPos) ||
				message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
				message.Author.IsBot)
				return;

			// Create a WebSocket-based command context based on the message
			var context = new SocketCommandContext(_client, message);

			// Execute the command with the command context we just
			// created, along with the service provider for precondition checks.

			// Keep in mind that result does not indicate a return value
			// rather an object stating if the command executed successfully.
			var result = await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: _serviceProvider);

			// Optionally, we may inform the user if the command fails
			// to be executed; however, this may not always be desired,
			// as it may clog up the request queue should a user spam a
			// command.
			// if (!result.IsSuccess)
			// await context.Channel.SendMessageAsync(result.ErrorReason);
		}

		public async Task StartAsync(CancellationToken token = default)
		{
			try
			{
				await InstallCommandsAsync();

				await _client.LoginAsync(TokenType.Bot, _discordConfiguration.Token);
				await _client.StartAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to connect to Discord");
				throw ex;
			}
		}

		public async Task StopAsync(CancellationToken token = default)
		{
			try
			{
				await _client.StopAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to disconnect from Discord");
				throw ex;
			}
		}

		private Task OnDiscordLog(LogMessage arg)
		{
			if (arg.Severity == LogSeverity.Critical)
			{
				_logger.Log(LogLevel.Critical, arg.Exception, "({0}) {1}", arg.Source, arg.Message);
			}
			else if (arg.Severity == LogSeverity.Debug)
			{
				_logger.Log(LogLevel.Debug, arg.Exception, "({0}) {1}", arg.Source, arg.Message);
			}
			else if (arg.Severity == LogSeverity.Error)
			{
				_logger.Log(LogLevel.Error, arg.Exception, "({0}) {1}", arg.Source, arg.Message);
			}
			else if (arg.Severity == LogSeverity.Info)
			{
				_logger.Log(LogLevel.Information, arg.Exception, "({0}) {1}", arg.Source, arg.Message);
			}
			else if (arg.Severity == LogSeverity.Verbose)
			{
				_logger.Log(LogLevel.Trace, arg.Exception, "({0}) {1}", arg.Source, arg.Message);
			}
			else if (arg.Severity == LogSeverity.Warning)
			{
				_logger.Log(LogLevel.Warning, arg.Exception, "({0}) {1}", arg.Source, arg.Message);
			}

			return Task.CompletedTask;
		}
	}
}
