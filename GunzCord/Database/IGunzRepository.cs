using GunzCord.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.Database
{
	public interface IGunzRepository
	{
		Task<Character> GetCharacterByNameAsync(string name);

		Task<Clan> GetClanInfoByCLIDAsync(int clid);

		Task<Clan> GetClanInfoByNameAsync(string name);

		Task<IEnumerable<ClanRanking>> GetClanRankingAsync();

		Task<IEnumerable<ServerStatus>> GetServerStatusAsync();
	}
}
