﻿namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public class MongoDbContextSettings
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }

        public MongoDbContextSettings(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }
    }
}
