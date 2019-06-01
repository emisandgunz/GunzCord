using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace GunzCord.Database.SQLite
{
	public class SQLiteDatabaseService : IDatabaseService
	{
		private readonly ILogger<SQLiteDatabaseService> _logger;
		private SQLiteConnection _connection;

		public IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		public string ConnectionString { get; protected set; }

		public SQLiteDatabaseService(IConfigurationRoot configuration, ILogger<SQLiteDatabaseService> logger)
		{
			ConnectionString = configuration.GetConnectionString("GunzDB");
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			if (_connection == null)
			{
				_logger.LogInformation("Starting database connection");

				try
				{
					_connection = new SQLiteConnection(ConnectionString);
					await _connection.OpenAsync();

					_logger.LogInformation("Database connected successfully");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Unable to connect to database");
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
				_logger.LogInformation("Closing database connection");

				try
				{
					if (_connection.State == ConnectionState.Open)
					{
						_connection.Close();
					}

					_logger.LogInformation("Database connection closed successfully");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Unable to close database connection");
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
