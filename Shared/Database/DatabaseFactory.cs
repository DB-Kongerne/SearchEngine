using System;

namespace Shared.Database
{
    /// <summary>
    /// Factory class for creating database instances.
    /// This is the single entry point for database access across all projects.
    /// </summary>
    public static class DatabaseFactory
    {
        public enum DatabaseType
        {
            Sqlite,
            Postgres
        }

        private static DatabaseType _defaultDbType = DatabaseType.Sqlite;

        /// <summary>
        /// Sets the default database type to use when creating a database instance
        /// </summary>
        public static void SetDefaultDatabaseType(DatabaseType dbType)
        {
            _defaultDbType = dbType;
        }

        /// <summary>
        /// Creates a database instance using the default database type
        /// </summary>
        public static IDatabase CreateDatabase()
        {
            return CreateDatabase(_defaultDbType);
        }

        /// <summary>
        /// Creates a database instance of the specified type
        /// </summary>
        public static IDatabase CreateDatabase(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.Sqlite:
                    return new DatabaseSqlite();
                case DatabaseType.Postgres:
                    return new DatabasePostgres();
                default:
                    throw new ArgumentException($"Unknown database type: {dbType}");
            }
        }

        /// <summary>
        /// Creates a database instance for indexing (creates tables)
        /// </summary>
        public static IDatabase CreateIndexingDatabase(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.Sqlite:
                    return new DatabaseSqliteIndexer();
                case DatabaseType.Postgres:
                    return new DatabasePostgresIndexer();
                default:
                    throw new ArgumentException($"Unknown database type: {dbType}");
            }
        }
    }
}
