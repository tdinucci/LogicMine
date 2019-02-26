using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine.DataObject.Ado.PostgreSql;
using Npgsql;
using Test.LogicMine.DataObject.Ado.PostgreSql.Util;
using Xunit;

namespace Test.LogicMine.DataObject.Ado.PostgreSql
{
    public class PostgreSqlInterfaceTest
    {
        [Fact]
        public async Task General()
        {
            const int recordCount = 10;
            using (var dbGenerator = new DbGenerator())
            {
                var dbInterface = new PostgreSqlInterface(dbGenerator.CreateDb());

                const string insertSql = "INSERT INTO Frog (Name, Date_Of_Birth) VALUES (@Name, @DateOfBirth);" +
                                         "SELECT CAST(lastval() AS integer);";

                const string readSingleSql = "SELECT * FROM Frog WHERE Id = @Id";
                const string readAllSql = "SELECT * FROM Frog";
                const string updateSql = "UPDATE Frog SET Name = @NewName WHERE Id BETWEEN @FromId AND @ToId";

                var ids = new List<int>();
                for (var i = 1; i <= recordCount; i++)
                {
                    var nameParam = new NpgsqlParameter("@Name", $"Kermit{i}");
                    var dateOfBirthParam = new NpgsqlParameter("@DateOfBirth", DateTime.Today.AddDays(-i));

                    var statement = new PostgreSqlStatement(insertSql, nameParam, dateOfBirthParam);
                    var id = await dbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false);

                    // it's a new DB so should be the case, depending on this for following tests
                    Assert.Equal(i, id);
                    ids.Add((int)id);
                }

                Assert.Equal(recordCount, ids.Count);

                foreach (var id in ids)
                {
                    var idParam = new NpgsqlParameter("@Id", id);
                    var statement = new PostgreSqlStatement(readSingleSql, idParam);

                    using (var reader = await dbInterface.GetReaderAsync(statement).ConfigureAwait(false))
                    {
                        Assert.True(await reader.ReadAsync().ConfigureAwait(false));
                        Assert.Equal(id, reader["Id"]);
                        Assert.Equal($"Kermit{id}", reader["Name"]);
                        Assert.Equal(DateTime.Today.AddDays(-id), Convert.ToDateTime(reader["date_of_birth"]));
                    }
                }

                var updateStatement = new PostgreSqlStatement(updateSql, new NpgsqlParameter("@NewName", "Frank"),
                    new NpgsqlParameter("@FromId", 5), new NpgsqlParameter("@ToId", 8));

                var affected = await dbInterface.ExecuteNonQueryAsync(updateStatement).ConfigureAwait(false);
                Assert.Equal(4, affected);

                var readAllStatement = new PostgreSqlStatement(readAllSql);
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