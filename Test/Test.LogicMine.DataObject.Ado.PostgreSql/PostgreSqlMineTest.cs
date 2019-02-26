using System;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.Common.LogicMine.Mine;
using Test.LogicMine.DataObject.Ado.PostgreSql.Util;

namespace Test.LogicMine.DataObject.Ado.PostgreSql
{
    public class PostgreSqlMineTest : MineTest<Frog<int>, int>, IDisposable
    {
        private DbGenerator _dbGenerator;

        protected override IDataObjectDescriptor GetDescriptor()
        {
            return new FrogDescriptor();
        }

        protected override IDataObjectStore<Frog<int>, int> GetObjectStore()
        {
            _dbGenerator = new DbGenerator();
            return new FrogObjectStore(_dbGenerator.CreateDb());
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