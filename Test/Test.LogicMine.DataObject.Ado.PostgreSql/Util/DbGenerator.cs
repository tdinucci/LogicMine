using System;
using System.IO;
using Npgsql;

namespace Test.LogicMine.DataObject.Ado.PostgreSql.Util
{
    public class DbGenerator
    {
        private const string DbName = "logic_mine_test";
        private const string ServerConnectionString = "Host=localhost;Username=postgres;Password=admin";

        private static readonly string DbConnectionString =
            $"Host=localhost;Username=postgres;Password=admin;Database={DbName}";

        public string CreateDb()
        {
            var createDbSql = $"CREATE DATABASE {DbName}";
            var createTableSql = @"
create table frog (
  id                serial        not null constraint email_pk primary key,
  name              varchar(255)  not null,
  date_of_birth     date          not null
);";

            if (!DbExists())
            {
                ExecuteStatement(ServerConnectionString, createDbSql);
                ExecuteStatement(DbConnectionString, createTableSql);
            }
            else
                EmptyTables();

            return DbConnectionString;
        }

        private void DeleteDb()
        {
            var deleteDbSql = $@"
select pg_terminate_backend (pg_stat_activity.pid)
from pg_stat_activity
where pg_stat_activity.datname = '{DbName}';
drop database if exists {DbName};";

            ExecuteStatement(ServerConnectionString, deleteDbSql);
        }

        private bool DbExists()
        {
            var query = $"SELECT * FROM pg_database WHERE datname = '{DbName}'";
            using (var conn = new NpgsqlConnection(ServerConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    return cmd.ExecuteReader().HasRows;
                }
            }
        }

        private void EmptyTables()
        {
            var statement = @"
DELETE FROM frog;
ALTER SEQUENCE frog_id_seq RESTART WITH 1;
UPDATE frog SET id=nextval('frog_id_seq');";

            ExecuteStatement(DbConnectionString, statement);
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
    }
}