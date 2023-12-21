using Microsoft.Data.Sqlite;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;

namespace CodingTracker
{
    internal class DatabaseManager
    {
        private string connectionString;
        private SqliteConnection connection;

        public DatabaseManager()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MySQLiteConnection"].ConnectionString;
            connection = new SqliteConnection(connectionString);
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public void CreateDatabase()
        {
            try
            {
                OpenConnection();

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
            finally
            {
                CloseConnection();
            }
        }

        public void ViewAllRecords()
        {
            OpenConnection();

            string selectQuery = "SELECT * FROM tracker";

            using (var command = new SqliteCommand(selectQuery, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("All Records in 'tracker' table:");
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0); // Assuming ID is the first column (index 0)
                            string startTime = reader.GetString(1); // Assuming StartTime is the second column (index 1)
                            string endTime = reader.GetString(2); // Assuming EndTime is the third column (index 2)
                            string duration = reader.GetString(3); // Assuming Duration is the fourth column (index 3)

                            Console.WriteLine($"ID: {id}, StartTime: {startTime}, EndTime: {endTime}, Duration: {duration}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No records found in 'tracker' table.");
                    }
                }
            }

            CloseConnection();
        }
    }
}
