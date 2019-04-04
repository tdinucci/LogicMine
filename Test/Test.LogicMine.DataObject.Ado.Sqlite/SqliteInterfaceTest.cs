using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogicMine.DataObject.Ado.Sqlite;
using Microsoft.Data.Sqlite;
using Test.LogicMine.DataObject.Ado.Mock;
using Test.LogicMine.DataObject.Ado.Sqlite.Util;
using Xunit;

namespace Test.LogicMine.DataObject.Ado.Sqlite
{
    public class SqliteInterfaceTest
    {
        private readonly string DbFilename = $"{Path.GetTempPath()}\\{Guid.NewGuid()}.db";

        [Fact]
        public async Task General()
        {
            const int recordCount = 10;
            using (var dbGenerator = new DbGenerator(DbFilename))
            {
                var dbInterface = new SqliteInterface(dbGenerator.CreateDb());

                const string insertSql = "INSERT INTO Frog (Name, DateOfBirth) VALUES (@Name, @DateOfBirth);" +
                                         "SELECT last_insert_rowid();";

                const string readSingleSql = "SELECT * FROM Frog WHERE Id = @Id";
                const string readAllSql = "SELECT * FROM Frog";
                const string updateSql = "UPDATE Frog SET Name = @NewName WHERE Id BETWEEN @FromId AND @ToId";

                var ids = new List<long>();
                for (long i = 1; i <= recordCount; i++)
                {
                    var nameParam = new SqliteParameter("@Name", $"Kermit{i}");
                    var dateOfBirthParam = new SqliteParameter("@DateOfBirth", DateTime.Today.AddDays(-i));

                    var statement = new SqliteStatement(insertSql, nameParam, dateOfBirthParam);
                    var id = await dbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false);

                    // it's a new DB so should be the case, depending on this for following tests
                    Assert.Equal(i, id);
                    ids.Add((long) id);
                }

                Assert.Equal(recordCount, ids.Count);

                foreach (var id in ids)
                {
                    var idParam = new SqliteParameter("@Id", id);
                    var statement = new SqliteStatement(readSingleSql, idParam);

                    using (var reader = await dbInterface.GetReaderAsync(statement).ConfigureAwait(false))
                    {
                        Assert.True(await reader.ReadAsync().ConfigureAwait(false));
                        Assert.Equal(id, reader["Id"]);
                        Assert.Equal($"Kermit{id}", reader["Name"]);
                        Assert.Equal(DateTime.Today.AddDays(-id), Convert.ToDateTime(reader["DateOfBirth"]));
                    }
                }

                var updateStatement = new SqliteStatement(updateSql, new SqliteParameter("@NewName", "Frank"),
                    new SqliteParameter("@FromId", 5), new SqliteParameter("@ToId", 8));

                var affected = await dbInterface.ExecuteNonQueryAsync(updateStatement).ConfigureAwait(false);
                Assert.Equal(4, affected);

                var readAllStatement = new SqliteStatement(readAllSql);
                using (var reader = await dbInterface.GetReaderAsync(readAllStatement).ConfigureAwait(false))
                {
                    var count = 0;
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        count++;
                        var id = (long) reader["Id"];
                        if (id >= 5 && id <= 8)
                            Assert.Equal("Frank", reader["Name"]);
                        else
                            Assert.Equal($"Kermit{id}", reader["Name"]);
                    }

                    Assert.Equal(recordCount, count);
                }
            }
        }

        [Fact]
        public async Task RetryContinuedFail()
        {
            using (var dbGenerator = new DbGenerator(DbFilename))
            {
                var dbInterface = new SqliteInterface(dbGenerator.CreateDb(),
                    new MockTransientErrorAwareExecutor("SQLite Error 1: 'no such table: NonTable'."));

                const string query = "SELECT * FROM NonTable";

                var statement = new SqliteStatement(query);
                var ex = await Assert
                    .ThrowsAsync<InvalidOperationException>(() => dbInterface.ExecuteScalarAsync(statement))
                    .ConfigureAwait(false);

                Assert.Equal("Failed after 3 attempts", ex.Message);
            }
        }

        [Fact]
        public async Task FailNoRetry()
        {
            using (var dbGenerator = new DbGenerator(DbFilename))
            {
                var dbInterface = new SqliteInterface(dbGenerator.CreateDb(),
                    new MockTransientErrorAwareExecutor("abcd"));

                const string query = "SELECT * FROM NonTable";

                var statement = new SqliteStatement(query);
                var ex = await Assert
                    .ThrowsAsync<SqliteException>(() => dbInterface.ExecuteScalarAsync(statement))
                    .ConfigureAwait(false);

                Assert.Equal("SQLite Error 1: 'no such table: NonTable'.", ex.Message);
            }
        }

        [Fact]
        public async Task RetryAndRecover()
        {
            var passOnThirdAttemptFunc = new Func<Task<object>>(() => Task.FromResult((object) 123));

            using (var dbGenerator = new DbGenerator(DbFilename))
            {
                var dbInterface = new SqliteInterface(dbGenerator.CreateDb(),
                    new MockTransientErrorAwareExecutor("SQLite Error 1: 'no such table: NonTable'.",
                        passOnThirdAttemptFunc));

                const string query = "SELECT * FROM NonTable";

                var statement = new SqliteStatement(query);
                var result = await dbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false);

                Assert.Equal(123, result);
            }
        }
    }
}