using System;
using System.Threading;
using Npgsql;

namespace Test.LogicMine.Api.Data.Postgres.Util
{
    public class DbGenerator : IDisposable
    {
        private const string DatabaseName = "logic_mine_test";
        private readonly string _connectionString;

        public DbGenerator()
        {
            _connectionString = "Host=localhost;Username=postgres;Password=admin";
        }

        public string CreateDb(string idFieldName = "id")
        {
            var createDbSql = "CREATE DATABASE logic_mine_test";
            var createTableSql = $@"
CREATE TABLE public.frog
(
    id serial NOT NULL,
    name text NOT NULL,
    date_of_birth date,
    PRIMARY KEY (id)
);";

            DeleteDb();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(createDbSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Pooled connections can go bad after table is dropped
            var dbConnectionString =
                $"Host=localhost;Username=postgres;Password=admin;Database={DatabaseName};Pooling=false";
            using (var conn = new NpgsqlConnection(dbConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(createTableSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return dbConnectionString;
        }

        public void DeleteDb()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand($@"
SELECT pg_terminate_backend (pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = '{DatabaseName}';
DROP DATABASE IF EXISTS {DatabaseName};", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            DeleteDb();
        }
    }
}
