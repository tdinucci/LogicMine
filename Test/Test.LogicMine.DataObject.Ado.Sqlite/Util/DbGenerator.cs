using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Test.LogicMine.DataObject.Ado.Sqlite.Util
{
    public class DbGenerator : IDisposable
    {
        private readonly string _filename;

        public DbGenerator(string filename)
        {
            _filename = filename;
        }

        public string CreateDb(string idFieldName = "Id")
        {
            var connectionString = $"Data Source={_filename}";
            var sql = $@"
CREATE TABLE Frog (
  {idFieldName} INTEGER     NOT NULL  PRIMARY KEY,
  Name          TEXT        NOT NULL,
  DateOfBirth   TEXT        NOT NULL
);";

            DeleteDb();
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return connectionString;
        }

        public void DeleteDb()
        {
            if (File.Exists(_filename))
                File.Delete(_filename);
        }

        public void Dispose()
        {
            DeleteDb();
        }
    }
}