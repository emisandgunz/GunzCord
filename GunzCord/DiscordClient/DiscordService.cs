using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GunzCord.Configuration;
using GunzCord.Database;
using GunzCord.Database.Events;
using GunzCord.Database.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.DiscordClient
{
	public class DiscordService : IDiscordService
	{
		private readonly IClanWarNotificationService _clanWarNotificationService;
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly DiscordConfiguration _discordConfiguration;
		private readonly GunZConfiguration _gunzConfiguration;
		private readonly IGunzRepository _gunzRepository;
		private readonly ILogger<DiscordService> _logger;
		private readonly IServiceProvider _serviceProvider;

		public DiscordService(
			IOptions<DiscordConfiguration> discordConfigurationOptions,
			IOptions<GunZConfiguration> gunzConfigurationOptions,
			IClanWarNotificationService clanWarNotificationService,
			IGunzRepository gunzRepository,
			ILogger<DiscordService> logger,
			IServiceProvider serviceProvider)
		{
			_discordConfiguration = discordConfigurationOptions.Value;
			_gunzConfiguration = gunzConfigurationOptions.Value;
			_clanWarNotificationService = clanWarNotificationService;
			_gunzRepository = gunzRepository;
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

		public async Task StartAsync(CancellationToken token = default)
		{
			try
			{
				await InstallCommandsAsync();

				await _client.LoginAsync(TokenType.Bot, _discordConfiguration.Token);
				await _client.StartAsync();

				if (_discordConfiguration.EnableClanWarNotifications)
				{
					if (_discordConfiguration.ServerId > 0 && _discordConfiguration.NotificationsChannelId > 0)
					{
						_clanWarNotificationService.OnClanWarNotification += OnClanWarNotificationAsync;

						await _clanWarNotificationService.StartAsync(token);
					}
					else if (_discordConfiguration.ServerId <= 0)
					{
						_logger.LogWarning("Discord Server ID is not configured, clan war notifications will not be sent");
					}
					else
					{
						_logger.LogWarning("Notifications Channel ID is not configured, clan war notifications will not be sent");
					}
				}
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
				if (_discordConfiguration.EnableClanWarNotifications)
				{
					await _clanWarNotificationService.StopAsync(token);
				}

				await _client.StopAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to disconnect from Discord");
				throw ex;
			}
		}

		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
			}
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

		private async Task InstallCommandsAsync()
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

		private async Task OnClanWarNotificationAsync(object sender, ClanWarNotificationEventArgs e)
		{
			if (_discordConfiguration.ServerId.HasValue 
				&& _discordConfiguration.NotificationsChannelId.HasValue 
				&& _discordConfiguration.NotificationsChannelId > 0)
			{
				var guild = _client.GetGuild(_discordConfiguration.ServerId.Value);

				if (guild != null)
				{
					var notificationsChannel = guild.GetTextChannel(_discordConfiguration.NotificationsChannelId.Value);

					if (notificationsChannel != null)
					{
						try
						{
							if (!_discordConfiguration.ShowClanWarNotificationsWithEmbed)
							{
								await notificationsChannel.SendMessageAsync(string.Format(Strings.CLAN_WAR_NOTIFICATION_MESSAGE, e.ClanGameLog.WinnerClanName, e.ClanGameLog.LoserClanName));
							}
							else
							{
								Clan winnerClan = await _gunzRepository.GetClanInfoByCLIDAsync(e.ClanGameLog.WinnerCLID);
								Clan loserClan = await _gunzRepository.GetClanInfoByCLIDAsync(e.ClanGameLog.LoserCLID);

								EmbedBuilder embed = null;

								if (winnerClan != null && loserClan != null)
								{
									string emblemUrl = _gunzConfiguration.DefaultClanEmblem;

									if (!string.IsNullOrEmpty(winnerClan.EmblemUrl))
									{
										emblemUrl = _gunzConfiguration.EmblemBaseUrl.EnsureEndsWith('/') + winnerClan.EmblemUrl;
									}

									embed = new EmbedBuilder()
									{
										Color = Color.Blue,
										Title = string.Format(Strings.CLAN_WAR_NOTIFICATION_MESSAGE, e.ClanGameLog.WinnerClanName, e.ClanGameLog.LoserClanName),
										ThumbnailUrl = emblemUrl,
										Timestamp = DateTime.UtcNow
									};

									embed.AddField(
										string.Format(Strings.CLAN_NAME_RANKING, e.ClanGameLog.WinnerClanName), 
										string.Format("{0} ({1} {2})", winnerClan.Ranking > 0 ? winnerClan.Ranking.ToString() : Strings.CLAN_UNRANKED, winnerClan.Point, Strings.CLAN_POINTS), 
										true);

									embed.AddField(
										string.Format(Strings.CLAN_NAME_RANKING, e.ClanGameLog.LoserClanName),
										string.Format("{0} ({1} {2})", loserClan.Ranking > 0 ? loserClan.Ranking.ToString() : Strings.CLAN_UNRANKED, loserClan.Point, Strings.CLAN_POINTS),
										true);

									embed.AddField(string.Format(Strings.CLAN_NAME_PLAYERS, e.ClanGameLog.WinnerClanName), e.ClanGameLog.WinnerMembers, true);
									embed.AddField(string.Format(Strings.CLAN_NAME_PLAYERS, e.ClanGameLog.LoserClanName), e.ClanGameLog.LoserMembers, true);
									embed.AddField(Strings.CLAN_ROUND_WINS, e.ClanGameLog.RoundWins, true);
									embed.AddField(Strings.CLAN_ROUND_LOSSES, e.ClanGameLog.RoundLosses, true);

									await notificationsChannel.SendMessageAsync(embed: embed.Build());
								}
								else
								{
									await notificationsChannel.SendMessageAsync(string.Format(Strings.CLAN_WAR_NOTIFICATION_MESSAGE, e.ClanGameLog.WinnerClanName, e.ClanGameLog.LoserClanName));
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Could not send a Clan War notification message");
						}
					}
					else
					{
						_logger.LogError("Could not find a Discord Channel for Notifications with the configured ID");
					}
				}
				else
				{
					_logger.LogError("Could not find a Discord Server with the configured ID");
				}
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
