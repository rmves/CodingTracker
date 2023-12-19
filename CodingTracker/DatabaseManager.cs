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

        }
    }
}
