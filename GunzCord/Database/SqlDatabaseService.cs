using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Database
{
	public class SqlDatabaseService : IDatabaseService
	{
		private readonly string _connectionString;
		private readonly ILogger _logger;

		private SqlConnection _connection;

		public IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		public SqlDatabaseService(IConfigurationRoot configuration, ILogger<SqlDatabaseService> logger)
		{
			_connectionString = configuration.GetConnectionString("GunzDB");
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			if (_connection == null)
			{
				_logger.LogInformation("[{0}] Starting database connection", DateTime.Now);

				try
				{
					_connection = new SqlConnection(_connectionString);
					await _connection.OpenAsync();

					_logger.LogInformation("[{0}] Database connected successfully", DateTime.Now);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "[{0}] Unable to connect to database", DateTime.Now);
					throw ex;
				}
			}
			else
			{
				throw new InvalidOperationException("Database connection already started");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken = default)
		{
			if (_connection != null)
			{
				_logger.LogInformation("[{0}] Closing database connection", DateTime.Now);

				try
				{
					if (_connection.State == ConnectionState.Open)
					{
						_connection.Close();
					}

					_logger.LogInformation("[{0}] Database connection closed successfully", DateTime.Now);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "[{0}] Unable to close database connection", DateTime.Now);
					throw ex;
				}
			}
			else
			{
				throw new InvalidOperationException("Database connection has not been started yet");
			}

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			if (_connection != null)
			{
				_connection.Dispose();
			}
		}
	}
}
