using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine.Api.Data.Postgres;
using Npgsql;
using Test.LogicMine.Api.Data.Postgres.Util;
using Xunit;

namespace Test.LogicMine.Api.Data.Postgres
{
  public class PostgresInterfaceTest
  {
    [Fact]
    public async Task General()
    {
      const int recordCount = 10;
      using (var dbGenerator = new DbGenerator())
      {
        var dbInterface = new PostgresInterface(dbGenerator.CreateDb());

        const string insertSql = "INSERT INTO frog (name, date_of_birth) VALUES (@Name, @DateOfBirth);" +
                                 "SELECT CAST(lastval() AS INT);";

        const string readSingleSql = "SELECT * FROM frog WHERE id = @Id";
        const string readAllSql = "SELECT * FROM frog";
        const string updateSql = "UPDATE frog SET name = @NewName WHERE id BETWEEN @FromId AND @ToId";

        var ids = new List<int>();
        for (var i = 1; i <= recordCount; i++)
        {
          var nameParam = new NpgsqlParameter("@Name", $"Kermit{i}");
          var dateOfBirthParam = new NpgsqlParameter("@DateOfBirth", DateTime.Today.AddDays(-i));

          var statement = new PostgresStatement(insertSql, nameParam, dateOfBirthParam);
          var id = (int) (await dbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false));

          // it's a new DB so should be the case, depending on this for following tests
          Assert.Equal(i, id);
          ids.Add(id);
        }

        Assert.Equal(recordCount, ids.Count);

        foreach (var id in ids)
        {
          var idParam = new NpgsqlParameter("@Id", id);
          var statement = new PostgresStatement(readSingleSql, idParam);

          using (var reader = await dbInterface.GetReaderAsync(statement).ConfigureAwait(false))
          {
            Assert.True(await reader.ReadAsync().ConfigureAwait(false));
            Assert.Equal(id, reader["id"]);
            Assert.Equal($"Kermit{id}", reader["name"]);
            Assert.Equal(DateTime.Today.AddDays(-id), Convert.ToDateTime(reader["date_of_birth"]));
          }
        }

        var updateStatement = new PostgresStatement(updateSql, new NpgsqlParameter("@NewName", "Frank"),
          new NpgsqlParameter("@FromId", 5), new NpgsqlParameter("@ToId", 8));

        var affected = await dbInterface.ExecuteNonQueryAsync(updateStatement).ConfigureAwait(false);
        Assert.Equal(4, affected);

        var readAllStatement = new PostgresStatement(readAllSql);
        using (var reader = await dbInterface.GetReaderAsync(readAllStatement).ConfigureAwait(false))
        {
          var count = 0;
          while (await reader.ReadAsync().ConfigureAwait(false))
          {
            count++;
            var id = (int) reader["id"];
            if (id >= 5 && id <= 8)
              Assert.Equal("Frank", reader["name"]);
            else
              Assert.Equal($"Kermit{id}", reader["name"]);
          }

          Assert.Equal(recordCount, count);
        }
      }
    }
  }
}
