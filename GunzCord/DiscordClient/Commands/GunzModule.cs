using Discord;
using Discord.Commands;
using GunzCord.Configuration;
using GunzCord.Database;
using GunzCord.Database.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.DiscordClient.Commands
{
	public class GunzModule : ModuleBase<SocketCommandContext>
	{
		private readonly AppConfiguration _appConfiguration;
		private readonly DiscordConfiguration _discordConfiguration;
		private readonly GunZConfiguration _gunzConfiguration;
		private readonly IGunzRepository _gunzRepository;
		private readonly ILogger<GunzModule> _logger;

		public GunzModule(
			IGunzRepository gunzRepository, 
			IOptions<AppConfiguration> appConfigurationOptions,
			IOptions<DiscordConfiguration> discordConfigurationOptions,
			IOptions<GunZConfiguration> gunzConfigurationOptions,
			ILogger<GunzModule> logger)
		{
			_gunzRepository = gunzRepository;
			_appConfiguration = appConfigurationOptions.Value;
			_discordConfiguration = discordConfigurationOptions.Value;
			_gunzConfiguration = gunzConfigurationOptions.Value;
			_logger = logger;
		}

		[Command("character")]
		[Alias("char", "charinfo", "characterinfo")]
		[Summary("Gets the specified character details")]
		public async Task CharacterInfoAsync(string name)
		{
			LogMessage();

			await Context.Channel.TriggerTypingAsync();

			var character = await _gunzRepository.GetCharacterByNameAsync(name);

			EmbedBuilder embed = null;

			if (character != null)
			{
				embed = new EmbedBuilder()
				{
					Color = Color.Blue,
					Description = Strings.CHARACTER_INFORMATION,
					Title = character.Name,
					Timestamp = DateTime.UtcNow
				};

				embed.AddField(Strings.CHARACTER_LEVEL, character.Level, true);
				embed.AddField(Strings.CHARACTER_XP, character.XP, true);

				if (!string.IsNullOrEmpty(character.ClanName))
				{
					embed.AddField(Strings.CHARACTER_CLAN, character.ClanName, true);
				}

				embed.AddField(Strings.CHARACTER_SEX, character.GetSexDisplayName(), true);
				embed.AddField(Strings.CHARACTER_GRADE, GetGradeDisplayName(character.UGradeID), true);
				embed.AddField(Strings.CHARACTER_KILLS, character.KillCount ?? 0, true);
				embed.AddField(Strings.CHARACTER_DEATHS, character.DeathCount ?? 0, true);
				embed.AddField(Strings.CHARACTER_KD, character.GetKillDeathRatio(), true);

				if (character.PlayTime.HasValue)
				{
					embed.AddField(Strings.CHARACTER_TIME_PLAYED, character.GetPlayTimeForDisplay());
				}

				if (character.RegDate.HasValue)
				{
					embed.AddField(Strings.CHARACTER_CREATED, character.RegDate.Value.ToString("G"), true);
				}

				if (character.LastTime.HasValue)
				{
					embed.AddField(Strings.CHARACTER_LAST_ONLINE, character.LastTime.Value.ToString("G"), true);
				}

				embed.WithFooter(_discordConfiguration.FooterSignature);
			}
			else
			{
				_logger.LogWarning("Unable to find a character with name \"{0}\"", name);

				embed = new EmbedBuilder()
				{
					Color = Color.Red,
					Description = string.Format(Strings.CHARACTER_INFORMATION_NOT_FOUND, name),
					Title = Strings.CHARACTER_INFORMATION,
					Timestamp = DateTime.UtcNow
				}
				.WithFooter(_discordConfiguration.FooterSignature);
			}

			await ReplyAsync(embed: embed.Build());
		}

		[Command("clan")]
		[Alias("claninfo")]
		[Summary("Gets the specified clan details")]
		public async Task ClanInfoAsync(string name)
		{
			LogMessage();

			await Context.Channel.TriggerTypingAsync();

			var clan = await _gunzRepository.GetClanInfoByNameAsync(name);

			EmbedBuilder embed = null;

			if (clan != null)
			{
				string emblemUrl = _gunzConfiguration.DefaultClanEmblem;

				if (!string.IsNullOrEmpty(clan.EmblemUrl))
				{
					emblemUrl = _gunzConfiguration.EmblemBaseUrl.EnsureEndsWith('/') + clan.EmblemUrl;
				}

				embed = new EmbedBuilder()
				{
					Color = Color.Blue,
					Description = Strings.CLAN_INFORMATION,
					Title = clan.Name,
					ThumbnailUrl = emblemUrl,
					Timestamp = DateTime.UtcNow
				};

				embed.AddField(Strings.CLAN_LEADER, clan.Leader, true);
				embed.AddField(Strings.CLAN_RANKING, clan.Ranking > 0 ? clan.Ranking.ToString() : Strings.CLAN_UNRANKED, true);
				embed.AddField(Strings.CLAN_POINTS, clan.Point, true);
				embed.AddField(Strings.CLAN_WINS, clan.Wins, true);
				embed.AddField(Strings.CLAN_LOSSES, clan.Losses, true);
				embed.AddField(Strings.CLAN_DRAWS, clan.Draws, true);
			}
			else
			{
				_logger.LogWarning("Unable to find a clan with name \"{0}\"", name);

				embed = new EmbedBuilder()
				{
					Color = Color.Red,
					Description = string.Format(Strings.CLAN_INFORMATION_NOT_FOUND, name),
					Title = Strings.CLAN_INFORMATION,
					Timestamp = DateTime.UtcNow
				}
				.WithFooter(_discordConfiguration.FooterSignature);
			}

			await ReplyAsync(embed: embed.Build());
		}

		[Command("clanranking")]
		[Alias("clanleaderboard", "clans")]
		[Summary("Gets the top 5 clans")]
		public async Task ClanLeaderboardAsync()
		{
			LogMessage();

			await Context.Channel.TriggerTypingAsync();

			var clanRanking = await _gunzRepository.GetClanRankingAsync();

			EmbedBuilder embed = null;

			if (clanRanking != null && clanRanking.Any())
			{
				embed = new EmbedBuilder()
				{
					Color = Color.Blue,
					Title = Strings.CLAN_LEADERBOARD,
					Timestamp = DateTime.UtcNow
				};

				int clanRank = 1;

				foreach (var clan in clanRanking)
				{
					embed.AddField($"{ clanRank }. { clan.Name }", $"{ clan.Point } { Strings.CLAN_POINTS }");

					clanRank++;
				}
			}
			else
			{
				embed = new EmbedBuilder()
				{
					Color = Color.Red,
					Description = Strings.CLAN_LEADERBOARD_NO_DATA,
					Title = Strings.CLAN_LEADERBOARD,
					Timestamp = DateTime.UtcNow
				}
				.WithFooter(_discordConfiguration.FooterSignature);
			}

			await ReplyAsync(embed: embed.Build());
		}

		[Command("help")]
		[Alias("commands", "ayuda")]
		[Summary("Gets the command list")]
		public async Task HelpAsync()
		{
			LogMessage();

			StringBuilder responseBuilder = new StringBuilder();

			responseBuilder.AppendLine($"**{ Strings.COMMANDS_TITLE }**");
			responseBuilder.AppendLine($"**{ _discordConfiguration.CommandPrefix }help** - { Strings.COMMANDS_HELP }");
			responseBuilder.AppendLine($"**{ _discordConfiguration.CommandPrefix }char <{ Strings.COMMANDS_NAME_PARAMETER }>** - { Strings.COMMANDS_CHARACTER }");
			responseBuilder.AppendLine($"**{ _discordConfiguration.CommandPrefix }clan <{ Strings.COMMANDS_NAME_PARAMETER }>** - { Strings.COMMANDS_CLAN }");
			responseBuilder.AppendLine($"**{ _discordConfiguration.CommandPrefix }clanranking** - { Strings.COMMANDS_CLANRANKING }");

			if (_appConfiguration.DatabaseType == DatabaseTypes.MICROSOFT_SQL_SERVER)
			{
				responseBuilder.AppendLine($"**{ _discordConfiguration.CommandPrefix }server** - { Strings.COMMANDS_SERVERINFO }");
				responseBuilder.AppendLine($"**{ _discordConfiguration.CommandPrefix }online** - { Strings.COMMANDS_ONLINE }");
			}

			await ReplyAsync(responseBuilder.ToString());
		}

		[Command("online")]
		[Alias("players")]
		[Summary("Gets the online player count")]
		public async Task OnlineAsync()
		{
			LogMessage();

			if (_appConfiguration.DatabaseType != DatabaseTypes.MICROSOFT_SQL_SERVER)
			{
				return;
			}

			await Context.Channel.TriggerTypingAsync();

			var serverStatus = await _gunzRepository.GetServerStatusAsync();

			int onlinePlayerCount = 0;

			foreach (var status in serverStatus)
			{
				onlinePlayerCount += status.CurrPlayer;
			}

			await ReplyAsync(string.Format(Strings.PLAYERS_ONLINE, onlinePlayerCount));
		}

		[Command("serverinfo")]
		[Alias("server", "servers", "status")]
		[Summary("Gets the server list")]
		public async Task ServerInfoAsync()
		{
			LogMessage();

			if (_appConfiguration.DatabaseType != DatabaseTypes.MICROSOFT_SQL_SERVER)
			{
				return;
			}

			await Context.Channel.TriggerTypingAsync();

			var serverStatus = await _gunzRepository.GetServerStatusAsync();

			var embed = new EmbedBuilder()
			{
				Color = Color.Blue,
				Title = Strings.SERVER_INFORMATION,
				Timestamp = DateTime.UtcNow
			};

			foreach (var server in serverStatus)
			{
				embed.AddField(server.ServerName, string.Format(Strings.SERVER_INFORMATION_DETAILS, server.CurrPlayer, server.MaxPlayer));
			}

			embed.WithFooter(_discordConfiguration.FooterSignature);

			await ReplyAsync(embed: embed.Build());
		}

		private string GetGradeDisplayName(int grade)
		{
			byte gradeByte = Convert.ToByte(grade);

			if (_gunzConfiguration.UGradeID_Admin.Contains(gradeByte))
			{
				return Strings.GRADE_ADMINISTRATOR;
			}
			else if (_gunzConfiguration.UGradeID_GameMaster.Contains(gradeByte))
			{
				return Strings.GRADE_GAME_MASTER;
			}
			else if (_gunzConfiguration.UGradeID_Event.Contains(gradeByte))
			{
				return Strings.GRADE_EVENT;
			}
			else if (_gunzConfiguration.UGradeID_Banned.Contains(gradeByte))
			{
				return Strings.GRADE_BANNED;
			}
			else if (_gunzConfiguration.UGradeID_Normal.Contains(gradeByte))
			{
				return Strings.GRADE_NORMAL;
			}

			return Strings.UNKNOWN;
		}

		private void LogMessage()
		{
			_logger.LogInformation("{0}: {1}", Context.Message.Author, Context.Message.Content);
		}
	}
}
