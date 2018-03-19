using System;
using System.Data.SqlClient;

namespace Test.LogicMine.Api.Data.Sql.Util
{
  public class DbGenerator : IDisposable
  {
    private readonly string _connectionString;

    public DbGenerator()
    {
      _connectionString = "Data Source=.;Initial Catalog=master;Persist Security Info=False;Integrated Security=true";
    }

    public string CreateDb(string idFieldName = "Id")
    {
      var createDbSql = "CREATE DATABASE LogicMineTest";
      var createTableSql = $@"
CREATE TABLE Frog (
  {idFieldName} INT           NOT NULL  PRIMARY KEY IDENTITY(1,1),
  Name          NVARCHAR(50)  NOT NULL,
  DateOfBirth   DATETIME      NOT NULL
);";

      DeleteDb();
      using (var conn = new SqlConnection(_connectionString))
      {
        conn.Open();
        using (var cmd = new SqlCommand(createDbSql, conn))
        {
          cmd.ExecuteNonQuery();
        }
      }

      var dbConnectionString =
        "Data Source=.;Initial Catalog=LogicMineTest;Persist Security Info=False;Integrated Security=true";
      using (var conn = new SqlConnection(dbConnectionString))
      {
        conn.Open();
        using (var cmd = new SqlCommand(createTableSql, conn))
        {
          cmd.ExecuteNonQuery();
        }
      }

      return dbConnectionString;
    }

    public void DeleteDb()
    {
      using (var conn = new SqlConnection(_connectionString))
      {
        conn.Open();
        using (var cmd = new SqlCommand(@"
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'LogicMineTest')
BEGIN
  ALTER DATABASE LogicMineTest SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE LogicMineTest
END", conn))
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
