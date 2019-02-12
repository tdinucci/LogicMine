using System;
using System.IO;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.Common.LogicMine.Mine;
using Test.LogicMine.DataObject.Ado.Sqlite.Util;

namespace Test.LogicMine.DataObject.Ado.Sqlite
{
    public class SqliteMineTest : MineTest<Frog<int>, int>, IDisposable
    {
        private readonly string DbFilename = $"{Path.GetTempPath()}\\{Guid.NewGuid()}.db";
        private DbGenerator _dbGenerator;

        protected override IDataObjectDescriptor GetDescriptor()
        {
            return new FrogDescriptor();
        }

        protected override IDataObjectStore<Frog<int>, int> GetObjectStore()
        {
            _dbGenerator = new DbGenerator(DbFilename);
            return new FrogObjectStore(_dbGenerator.CreateDb("FrogId"));
        }

        protected override Frog<int> CreateFrog(int index, string name, DateTime dateOfBirth)
        {
            return new Frog<int> {Name = name, DateOfBirth = dateOfBirth};
        }

        protected override void DeleteAll()
        {
        }

        public void Dispose()
        {
            _dbGenerator?.Dispose();
        }
    }
}