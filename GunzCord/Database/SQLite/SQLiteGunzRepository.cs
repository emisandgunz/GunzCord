using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using GunzCord.Database.Models;

namespace GunzCord.Database.SQLite
{
	public class SQLiteGunzRepository : IGunzRepository
	{
		private readonly IDatabaseService _databaseService;

		public SQLiteGunzRepository(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		public async Task<Character> GetCharacterByNameAsync(string name)
		{
			Character result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection
					.QueryFirstOrDefaultAsync<Character>(
					"SELECT [c].[AID], [c].[CID], [c].[Name], [c].[Level], [c].[Sex], [c].[XP], [c].[RegDate], [c].[LastTime], [c].[PlayTime], [c].[KillCount], [c].[DeathCount], [a].[UGradeID], [cl].[Name] AS [ClanName] " +
					"FROM 'Character' AS [c] " +
					"INNER JOIN 'Account' AS [a] ON [c].[AID] = [a].[AID] " +
					"LEFT OUTER JOIN 'ClanMember' AS [clm] ON [c].[CID] = [clm].[CID] " +
					"LEFT OUTER JOIN 'Clan' AS [cl] ON [clm].[CLID] = [cl].[CLID] " +
					"WHERE UPPER([c].[Name]) = UPPER(@Name) AND ([c].[DeleteFlag] = 0 OR [c].[DeleteFlag] IS NULL)",
					new { Name = name },
					transaction: transaction);
			}

			return result;
		}

		public async Task<Clan> GetClanInfoByCLIDAsync(int clid)
		{
			Clan result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection
					.QueryFirstOrDefaultAsync<Clan>(
					"SELECT [cl].[CLID], [cl].[Name], [cl].[MasterCID], [cl].[Point], [cl].[Wins], [cl].[RegDate], [cl].[Losses], [cl].[Draws], [cl].[Ranking], [cl].[EmblemUrl], [c].[Name] AS [Leader] " +
					"FROM 'Clan' AS [cl] " +
					"INNER JOIN 'Character' AS [c] ON [cl].[MasterCID] = [c].[CID] " +
					"WHERE [cl].[CLID] = @CLID) AND ([cl].[DeleteFlag] = 0 OR [cl].[DeleteFlag] IS NULL) AND ([c].[DeleteFlag] = 0 OR [c].[DeleteFlag] IS NULL)",
					new { CLID = clid },
					transaction: transaction);
			}

			return result;
		}

		public async Task<Clan> GetClanInfoByNameAsync(string name)
		{
			Clan result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection
					.QueryFirstOrDefaultAsync<Clan>(
					"SELECT [cl].[CLID], [cl].[Name], [cl].[MasterCID], [cl].[Point], [cl].[Wins], [cl].[RegDate], [cl].[Losses], [cl].[Draws], [cl].[Ranking], [cl].[EmblemUrl], [c].[Name] AS [Leader] " +
					"FROM 'Clan' AS [cl] " +
					"INNER JOIN 'Character' AS [c] ON [cl].[MasterCID] = [c].[CID] " +
					"WHERE UPPER([cl].[Name]) = UPPER(@Name) AND ([cl].[DeleteFlag] = 0 OR [cl].[DeleteFlag] IS NULL) AND ([c].[DeleteFlag] = 0 OR [c].[DeleteFlag] IS NULL)", 
					new { Name = name }, 
					transaction: transaction);
			}

			return result;
		}

		public async Task<IEnumerable<ClanRanking>> GetClanRankingAsync()
		{
			IEnumerable<ClanRanking> result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection.QueryAsync<ClanRanking>(
					"SELECT [cl].[CLID], [cl].[Name], [cl].[Point] " +
					"FROM 'Clan' AS [cl] " +
					"WHERE ([cl].[DeleteFlag] = 0 OR [cl].[DeleteFlag] IS NULL) AND [cl].[Ranking] > 0 " +
					"ORDER BY [cl].[Ranking] ASC " +
					"LIMIT 5", 
					transaction: transaction);
			}

			return result;
		}

		public Task<IEnumerable<ServerStatus>> GetServerStatusAsync()
		{
			return Task.FromResult((IEnumerable<ServerStatus>)null);
		}
	}
}
