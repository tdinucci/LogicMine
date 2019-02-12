using System;
using System.IO;
using System.Threading.Tasks;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.Ado.Sqlite.Util;

namespace Test.LogicMine.DataObject
{
    public class TerminalTest : IDisposable
    {
        private readonly string DbFilename = $"{Path.GetTempPath()}\\{Guid.NewGuid()}.db";
        private DbGenerator _dbGenerator;

        protected IDataObjectStore<Frog<int>, int> GetStore(int frogCount)
        {
            _dbGenerator = new DbGenerator(DbFilename);
            var store = new FrogObjectStore(_dbGenerator.CreateDb("FrogId"));

            InsertFrogs(store, frogCount);

            return store;
        }

        private void InsertFrogs(IDataObjectStore<Frog<int>, int> store, int count)
        {
            var tasks = new Task[count];
            for (var i = 1; i <= count; i++)
            {
                var frog = new Frog<int> {Id = i, Name = $"Frank{i}", DateOfBirth = DateTime.Today.AddDays(-i)};
                tasks[i - 1] = store.CreateAsync(frog);
            }

            Task.WaitAll(tasks);
        }

        public void Dispose()
        {
            _dbGenerator?.Dispose();
        }
    }
}