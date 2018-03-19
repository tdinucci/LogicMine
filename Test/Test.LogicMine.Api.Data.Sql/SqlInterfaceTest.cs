using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LogicMine.Api.Data.Sql;
using Test.LogicMine.Api.Data.Sql.Util;
using Xunit;

namespace Test.LogicMine.Api.Data.Sql
{
  public class SqlInterfaceTest
  {
    [Fact]
    public async Task General()
    {
      const int recordCount = 10;
      using (var dbGenerator = new DbGenerator())
      {
        var dbInterface = new SqlInterface(dbGenerator.CreateDb());

        const string insertSql = "INSERT INTO Frog (Name, DateOfBirth) VALUES (@Name, @DateOfBirth);" +
                                 "SELECT CAST(SCOPE_IDENTITY() AS INT);";

        const string readSingleSql = "SELECT * FROM Frog WHERE Id = @Id";
        const string readAllSql = "SELECT * FROM Frog";
        const string updateSql = "UPDATE Frog SET Name = @NewName WHERE Id BETWEEN @FromId AND @ToId";

        var ids = new List<int>();
        for (var i = 1; i <= recordCount; i++)
        {
          var nameParam = new SqlParameter("@Name", $"Kermit{i}");
          var dateOfBirthParam = new SqlParameter("@DateOfBirth", DateTime.Today.AddDays(-i));

          var statement = new SqlStatement(insertSql, nameParam, dateOfBirthParam);
          var id = (int)(await dbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false));

          // it's a new DB so should be the case, depending on this for following tests
          Assert.Equal(i, id);
          ids.Add(id);
        }

        Assert.Equal(recordCount, ids.Count);

        foreach (var id in ids)
        {
          var idParam = new SqlParameter("@Id", id);
          var statement = new SqlStatement(readSingleSql, idParam);

          using (var reader = await dbInterface.GetReaderAsync(statement).ConfigureAwait(false))
          {
            Assert.True(await reader.ReadAsync().ConfigureAwait(false));
            Assert.Equal(id, reader["Id"]);
            Assert.Equal($"Kermit{id}", reader["Name"]);
            Assert.Equal(DateTime.Today.AddDays(-id), Convert.ToDateTime(reader["DateOfBirth"]));
          }
        }

        var updateStatement = new SqlStatement(updateSql, new SqlParameter("@NewName", "Frank"),
          new SqlParameter("@FromId", 5), new SqlParameter("@ToId", 8));

        var affected = await dbInterface.ExecuteNonQueryAsync(updateStatement).ConfigureAwait(false);
        Assert.Equal(4, affected);

        var readAllStatement = new SqlStatement(readAllSql);
        using (var reader = await dbInterface.GetReaderAsync(readAllStatement).ConfigureAwait(false))
        {
          var count = 0;
          while (await reader.ReadAsync().ConfigureAwait(false))
          {
            count++;
            var id = (int) reader["Id"];
            if (id >= 5 && id <= 8)
              Assert.Equal("Frank", reader["Name"]);
            else
              Assert.Equal($"Kermit{id}", reader["Name"]);
          }

          Assert.Equal(recordCount, count);
        }
      }
    }
  }
}
