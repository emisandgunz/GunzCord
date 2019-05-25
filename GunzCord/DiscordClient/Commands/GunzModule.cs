using Discord;
using Discord.Commands;
using GunzCord.Configuration;
using GunzCord.Database;
using GunzCord.Database.Models;
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
		private readonly DiscordConfiguration _discordConfiguration;
		private readonly GunZConfiguration _gunzConfiguration;
		private readonly IGunzRepository _gunzRepository;

		public GunzModule(
			IGunzRepository gunzRepository, 
			IOptions<DiscordConfiguration> discordConfigurationOptions,
			IOptions<GunZConfiguration> gunzConfigurationOptions)
		{
			_gunzRepository = gunzRepository;
			_discordConfiguration = discordConfigurationOptions.Value;
			_gunzConfiguration = gunzConfigurationOptions.Value;
		}

		[Command("character")]
		[Alias("char", "charinfo", "characterinfo")]
		[Summary("Gets the specified character details")]
		public async Task CharacterInfo(string name)
		{
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

		[Command("online")]
		[Alias("players")]
		[Summary("Gets the online player count")]
		public async Task OnlineAsync()
		{
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
		[Alias("server", "status")]
		[Summary("Gets the server list")]
		public async Task ServerInfoAsync()
		{
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
	}
}
