using System;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.Ado.PostgreSql.Util;

namespace Test.LogicMine.DataObject.Ado.PostgreSql
{
    public class PostgreSqlMappedObjectStoreTest : MappedObjectStoreTest<Frog<int>, int>
    {
        private DbGenerator _dbGenerator;

        protected override IDataObjectStore<Frog<int>, int> GetStore()
        {
            _dbGenerator = new DbGenerator();
            return new FrogObjectStore(_dbGenerator.CreateDb());
        }

        protected override Frog<int> CreateFrog(int index, string name, DateTime dateOfBirth)
        {
            return new Frog<int> {Id = index, Name = name, DateOfBirth = dateOfBirth};
        }

        protected override void DeleteAll()
        {
        }
    }
}