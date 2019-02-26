using System;
using System.IO;
using Npgsql;

namespace Test.LogicMine.DataObject.Ado.PostgreSql.Util
{
    public class DbGenerator : IDisposable
    {
        private const string DbName = "logic_mine_test";
        private const string ServerConnectionString = "Host=localhost;Username=;Password=admin";

        // Pooled connections can go bad after table is dropped
        private static readonly string DbConnectionString =
            $"Host=localhost;Username=postgres;Password=;Database={DbName};Pooling=false";

        public string CreateDb()
        {
            var createDbSql = $"CREATE DATABASE {DbName}";
            var createTableSql = @"
create table frog (
  id                serial        not null constraint email_pk primary key,
  name              varchar(255)  not null,
  date_of_birth     date          not null
);";

            DeleteDb();
            ExecuteStatement(ServerConnectionString, createDbSql);
            ExecuteStatement(DbConnectionString, createTableSql);

            return DbConnectionString;
        }

        public void DeleteDb()
        {
            var deleteDbSql = $@"
select pg_terminate_backend (pg_stat_activity.pid)
from pg_stat_activity
where pg_stat_activity.datname = '{DbName}';
drop database if exists {DbName};";

            ExecuteStatement(ServerConnectionString, deleteDbSql);
        }

        private void ExecuteStatement(string connectionString, string statement)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(statement, conn))
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