using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;

namespace CodingTracker
{
    internal class DatabaseManager
    {
        private readonly string connectionString ;

        public DatabaseManager()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddXmlFile("app.config", optional: true, reloadOnChange: true)
                .Build();

            var connectionStrings = config.GetSection("connectionStrings");

            if (connectionStrings != null )
            {
                throw new InvalidOperationException("ConnectionStrings section not found in app.config.");
            }

            var firstConnectionString = connectionStrings.GetChildren().FirstOrDefault();
            string connStrName = firstConnectionString?.Key;

            if (string.IsNullOrEmpty(connStrName))
            {
                throw new InvalidOperationException("No connection string found in app.config.");
            }

            string connString = firstConnectionString?.Value;

            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException($"Connection string '{connStrName}' not found in app.config.");
            }

            var builder = new SqliteConnectionStringBuilder(connString);
            string dbName = builder.DataSource;

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);
            connectionString = $"Data Source={dbPath}";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(connectionString)) // Check if the database file exists
            {
                // If the database doesn't exist, create it
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // SQL command to create a table (replace this with your table creation script)
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS tracker (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            StartTime TEXT,
                            EndTime TEXT,
                            Duration TEXT
                        )";

                    using (var command = new SqliteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
