using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Data;
using System.Globalization;
using ConsoleTableExt;

namespace CodingTracker
{
    internal static class DatabaseManager
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["MySQLiteConnection"].ConnectionString;
        private static SqliteConnection GetOpenConnection()
        {
            var connection = new SqliteConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        public static void CreateDatabase()
        {
            try
            {
                using (var connection = GetOpenConnection())
                {
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
            }

                
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to SQLite database: " + ex.Message);
                // Handle the exception or log it as needed
            }
        }

        public static string CalculateDuration(string startTime, string endTime)
        {
            // Assuming the time format is HH:mm:ss
            DateTime start = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);

            // Calculating the duration
            TimeSpan duration = end - start;

            // Returning the duration as a formatted string
            return duration.ToString(@"hh\:mm\:ss");
        }

        public static void ViewAllRecords()
        {
            try
            {
                using (var connection = GetOpenConnection())
                {
                    string selectQuery = "SELECT * FROM tracker";

                    using (var command = new SqliteCommand(selectQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                var tableData = new List<List<object>>();

                                var columnNames = Enumerable.Range(0, reader.FieldCount)
                                    .Select(reader.GetName)
                                    .ToList();

                                tableData.Add(columnNames.Cast<object>().ToList());

                                while (reader.Read())
                                {
                                    var rowData = new List<object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        rowData.Add(reader.GetValue(i));
                                    }

                                    tableData.Add(rowData);
                                }
                                Console.WriteLine("All records in 'tracker' table");
                                ConsoleTableBuilder.From(tableData).ExportAndWriteLine();
                            }
                            else
                            {
                                Console.WriteLine("No records found in 'tracker' table.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error viewing records: " + ex.Message);
                // Handle the exception or log it as needed
            }
        }

        public static void Insert()
        {
            using (var connection = GetOpenConnection())
            {
                Console.WriteLine("Please enter the time in the format: HH:mm ");
                string startTime = UserInput.GetStringInput("Enter start time: ");
                string endTime = UserInput.GetStringInput("Enter end time: ");

                if (!IsValidTimeFormat(startTime) || !IsValidTimeFormat(endTime))
                {
                    Console.WriteLine("Invlaid time format. Please enter the time in the format: HH:mm");
                    MainMenu.ReturnToMainMenu();
                    return;
                }

                DateTime currentDateUtc = DateTime.UtcNow.Date;

                DateTime startTimeWithDate = currentDateUtc.Add(TimeSpan.Parse(startTime));
                DateTime endTimeWithDate = currentDateUtc.Add(TimeSpan.Parse(endTime));


                string duration = CalculateDuration(startTimeWithDate.ToString(), endTimeWithDate.ToString());

                string insertQuery = "INSERT INTO tracker (StartTime, EndTime, Duration) VALUES (@startTime, @endTime, @duration)";
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@startTime", startTimeWithDate);
                    command.Parameters.AddWithValue("@endTime", endTimeWithDate);
                    command.Parameters.AddWithValue("@duration", duration);

                    command.ExecuteNonQuery();
                    Console.WriteLine("Data inserted successfully!");
                }
            }

            MainMenu.ReturnToMainMenu();
        }
        private static bool IsValidTimeFormat(string time)
        {
            // Check if the provided time string matches the specified format
            return DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        public static void Update()
        {
            ViewAllRecords();

            var connection = GetOpenConnection();

            try
            {
                int recordIdToUpdate;

                while (true)
                {
                    Console.Write("Enter the ID of the record to update (0 to cancel): ");
                    string input = Console.ReadLine();

                    if (input == "0")
                    {
                        MainMenu.ReturnToMainMenu();
                        return;
                    }

                    if (int.TryParse(input, out recordIdToUpdate))
                    {
                        // Check if the record with the provided ID exists
                        string checkRecordQuery = "SELECT COUNT(*) FROM tracker WHERE ID = @recordIdToUpdate";

                        using (var checkCommand = new SqliteCommand(checkRecordQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@recordIdToUpdate", recordIdToUpdate);
                            int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                            if (recordCount == 1)
                                break;

                            Console.WriteLine($"Record with ID {recordIdToUpdate} does not exist.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid integer.");
                    }
                }

                // Fetch the record with the given ID to display existing StartTime and EndTime
                string selectQuery = "SELECT StartTime, EndTime FROM tracker WHERE ID = @recordIdToUpdate";
                string startTime = "";
                string endTime = "";

                using (var selectCommand = new SqliteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@recordIdToUpdate", recordIdToUpdate);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            startTime = reader.GetString(0);
                            endTime = reader.GetString(1);
                        }
                    }
                }

                Console.Clear();
                Console.WriteLine("Current Record Details:");
                var tableData = new List<List<object>>();
                tableData.Add(new List<object> { "StartTime", "EndTime", "Duration" });
                tableData.Add(new List<object> { startTime, endTime, CalculateDuration(startTime, endTime) });

                ConsoleTableBuilder
                    .From(tableData)
                    .WithFormat(ConsoleTableBuilderFormat.Alternative)
                    .ExportAndWriteLine();

                // Get user input for field to update
                Console.WriteLine("Which field do you want to update?");
                Console.WriteLine("1 - StartTime");
                Console.WriteLine("2 - EndTime");

                int fieldChoice = UserInput.GetIntegerInput("Enter the number for the field to update: ");

                string updateField = (fieldChoice == 1) ? "StartTime" : "EndTime";

                // Prompt the user with the required format for updating the field
                Console.WriteLine($"Please enter the new {updateField} in the format: yyyy-MM-dd HH:mm:ss");

                // Get new time value from user
                string newTime = UserInput.GetStringInput($"Enter the new {updateField}: ");

                // Validate and convert newTime to the format matching the database
                DateTime parsedTime;
                if (!DateTime.TryParseExact(newTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTime))
                {
                    Console.WriteLine($"Invalid format for {updateField}. Please use the format: yyyy-MM-dd HH:mm:ss");
                    MainMenu.ReturnToMainMenu();
                    return;
                }

                newTime = parsedTime.ToString("yyyy-MM-dd HH:mm:ss");

                //Calculate updated duration based on new time
                string updatedDuration = CalculateDuration((updateField == "StartTime") ? newTime : startTime, (updateField == "EndTime") ? newTime : endTime);

                // Perform the update query
                string updateQuery = $"UPDATE tracker SET {updateField} = @newTime, Duration = @updatedDuration WHERE ID = @recordIdToUpdate";

                using (var updateCommand = new SqliteCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@newTime", newTime);
                    updateCommand.Parameters.AddWithValue("@updatedDuration", updatedDuration);
                    updateCommand.Parameters.AddWithValue("@recordIdToUpdate", recordIdToUpdate);

                    updateCommand.ExecuteNonQuery();

                    Console.WriteLine("Record updated successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating record: " + ex.Message);
                // Handle the exception or log it as needed
            }
            finally
            {
                connection.Close();
                MainMenu.ReturnToMainMenu();
            }
        }

        public static void Delete()
        {
            ViewAllRecords();

            var connection = GetOpenConnection();

            try
            {
                int recordIdToDelete;

                while (true)
                {
                    Console.Write("Enter the ID of the record to delete (0 to cancel): ");
                    string input = Console.ReadLine();

                    if (input == "0")
                    {
                        MainMenu.ReturnToMainMenu();
                        return;
                    }

                    if (int.TryParse(input, out recordIdToDelete))
                    {
                        // Check if the record with the provided ID exists
                        string checkRecordQuery = "SELECT COUNT(*) FROM tracker WHERE ID = @recordIdToDelete";

                        using (var checkCommand = new SqliteCommand(checkRecordQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@recordIdToDelete", recordIdToDelete);
                            int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                            if (recordCount == 1)
                                break;

                            Console.WriteLine($"Record with ID {recordIdToDelete} does not exist.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid integer.");
                    }
                }

                // Perform the delete query
                string deleteQuery = "DELETE FROM tracker WHERE ID = @recordIdToDelete";

                using (var deleteCommand = new SqliteCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@recordIdToDelete", recordIdToDelete);

                    deleteCommand.ExecuteNonQuery();
                    Console.WriteLine("Record deleted successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting record: " + ex.Message);
                // Handle the exception or log it as needed
            }
            finally
            {
                connection.Close();
                MainMenu.ReturnToMainMenu();
            }
        }

    }
}

