using Dapper;
using GunzCord.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GunzCord.Database.SqlServer
{
	public class SqlGunzRepository : IGunzRepository
	{
		private readonly IDatabaseService _databaseService;

		public SqlGunzRepository(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		public async Task<Character> GetCharacterByNameAsync(string name)
		{
			Character result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection
					.QueryFirstOrDefaultAsync<Character>("[dbo].[spDiscordGetCharInfoByName]", new { Name = name }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
			}

			return result;
		}

		public async Task<Clan> GetClanInfoByNameAsync(string name)
		{
			Clan result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection
					.QueryFirstOrDefaultAsync<Clan>("[dbo].[spDiscordGetClanInfoByName]", new { Name = name }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
			}

			return result;
		}

		public async Task<IEnumerable<ClanRanking>> GetClanRankingAsync()
		{
			IEnumerable<ClanRanking> result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection.QueryAsync<ClanRanking>("[dbo].[spDiscordGetClanRanking]", transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
			}

			return result;
		}

		public async Task<IEnumerable<ServerStatus>> GetServerStatusAsync()
		{
			IEnumerable<ServerStatus> result = null;

			using (var transaction = _databaseService.Connection.BeginTransaction())
			{
				result = await _databaseService.Connection.QueryAsync<ServerStatus>("[dbo].[spDiscordGetServerInfo]", transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
			}

			return result;
		}
	}
}
