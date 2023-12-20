using Microsoft.Data.Sqlite;
using System.Collections.Specialized;
using System.Configuration;

namespace CodingTracker
{
    internal class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager()
        {
            // Retrieve the connection string named "MySQLiteConnection" from app.config
            connectionString = ConfigurationManager.ConnectionStrings["MySQLiteConnection"].ConnectionString;
        }

        public void CreateDatabase()
        {
            // Use the retrieved connection string to create a SQLite database
            using (var connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Perform any database initialization or setup here if needed
                    Console.WriteLine("Connected to SQLite database successfully!");

                    // Create the 'tracker' table if it doesn't exist
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
                        Console.WriteLine("Tracker table created successfully!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting to SQLite database: " + ex.Message);
                    // Handle the exception or log it as needed
                }
            }
        }
    }
}
