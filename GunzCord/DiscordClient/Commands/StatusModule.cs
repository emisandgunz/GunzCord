using Discord;
using Discord.Commands;
using GunzCord.Configuration;
using GunzCord.Database;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.DiscordClient.Commands
{
	public class StatusModule : ModuleBase<SocketCommandContext>
	{
		private readonly DiscordConfiguration _discordConfiguration;
		private readonly IGunzRepository _gunzRepository;

		public StatusModule(IGunzRepository gunzRepository, IOptions<DiscordConfiguration> discordConfigurationOptions)
		{
			_gunzRepository = gunzRepository;
			_discordConfiguration = discordConfigurationOptions.Value;
		}

		[Command("online")]
		[Summary("Gets the online player count")]
		public async Task OnlineAsync()
		{
			var serverStatus = await _gunzRepository.GetServerStatusAsync();

			int onlinePlayerCount = 0;

			foreach (var status in serverStatus)
			{
				onlinePlayerCount += status.CurPlayer;
			}

			await ReplyAsync($"There are currently { onlinePlayerCount } players online!");
		}

		[Command("serverinfo")]
		[Summary("Gets the server list")]
		public async Task ServerInfoAsync()
		{
			await Context.Channel.TriggerTypingAsync();

			var serverStatus = await _gunzRepository.GetServerStatusAsync();

			var embed = new EmbedBuilder()
			{
				Color = new Color(Color.Blue.RawValue),
				Title = "Server Information",
				Timestamp = DateTime.UtcNow
			};

			foreach (var server in serverStatus)
			{
				embed.AddField(server.Name, $"{ server.CurPlayer }/{ server.MaxPlayer } players online");
			}

			embed.WithFooter(_discordConfiguration.FooterSignature);

			await ReplyAsync(embed: embed.Build());
		}
	}
}
