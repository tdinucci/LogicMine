using System;
using System.IO;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado;
using LogicMine.DataObject.Ado.Sqlite;
using Test.Common.LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.Ado.Sqlite.Util;

namespace Test.LogicMine.DataObject.Ado.Sqlite
{
    public class SqliteMappedObjectStoreTest : MappedObjectStoreTest<Frog<int>, int>, IDisposable
    {
        private static readonly string DbFilename = $"{Path.GetTempPath()}\\testm.db";
        private DbGenerator _dbGenerator;

        protected override IDataObjectStore<Frog<int>, int> GetStore()
        {
            _dbGenerator = new DbGenerator(DbFilename);
            return new FrogObjectStore(_dbGenerator.CreateDb("FrogId"));
        }

        protected override Frog<int> CreateFrog(int index, string name, DateTime dateOfBirth)
        {
            return new Frog<int> {Id = index, Name = name, DateOfBirth = dateOfBirth};
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