using System;
using System.Collections.Generic;
using LogicMine.DataObject.Ado.PostgreSql;
using Npgsql;
using Test.Common.LogicMine;
using Test.LogicMine.DataObject.Ado.PostgreSql.Util;
using Xunit;

namespace Test.LogicMine.DataObject.Ado.PostgreSql
{
    public class PostgreSqlInterfaceTest
    {
        [Fact]
        public void General()
        {
            lock (GlobalLocker.Lock)
            {
                const int recordCount = 10;
                var dbGenerator = new DbGenerator();
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
                    var id = dbInterface.ExecuteScalarAsync(statement).GetAwaiter().GetResult();

                    // it's a new DB so should be the case, depending on this for following tests
                    Assert.Equal(i, id);
                    ids.Add((int) id);
                }

                Assert.Equal(recordCount, ids.Count);

                foreach (var id in ids)
                {
                    var idParam = new NpgsqlParameter("@Id", id);
                    var statement = new PostgreSqlStatement(readSingleSql, idParam);

                    using (var reader = dbInterface.GetReaderAsync(statement).GetAwaiter().GetResult())
                    {
                        Assert.True(reader.Read());
                        Assert.Equal(id, reader["Id"]);
                        Assert.Equal($"Kermit{id}", reader["Name"]);
                        Assert.Equal(DateTime.Today.AddDays(-id), Convert.ToDateTime(reader["date_of_birth"]));
                    }
                }

                var updateStatement = new PostgreSqlStatement(updateSql, new NpgsqlParameter("@NewName", "Frank"),
                    new NpgsqlParameter("@FromId", 5), new NpgsqlParameter("@ToId", 8));

                var affected = dbInterface.ExecuteNonQueryAsync(updateStatement).GetAwaiter().GetResult();
                Assert.Equal(4, affected);

                var readAllStatement = new PostgreSqlStatement(readAllSql);
                using (var reader = dbInterface.GetReaderAsync(readAllStatement).GetAwaiter().GetResult())
                {
                    var count = 0;
                    while (reader.Read())
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