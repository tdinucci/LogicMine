using System;
using LogicMine.DataObject;
using MongoDB.Driver;
using Test.Common.LogicMine.DataType;
using Test.Common.LogicMine.Mine;
using Test.LogicMine.DataObject.MongoDb.Util;

namespace Test.LogicMine.DataObject.MongoDb
{
    public class MongoDbMineTest : MineTest<Frog<Guid>, Guid>
    {
        private readonly IMongoDatabase _db;

        public MongoDbMineTest()
        {
            _db = new MongoClient("mongodb://localhost:27017").GetDatabase("logicMineTest");
        }

        protected override IDataObjectDescriptor GetDescriptor()
        {
            return new FrogDescriptor();
        }

        protected override IDataObjectStore<Frog<Guid>, Guid> GetObjectStore()
        {
            return new FrogObjectStore(_db);
        }

        protected override Frog<Guid> CreateFrog(int index, string name, DateTime dateOfBirth)
        {
            return new Frog<Guid> {Name = name, DateOfBirth = dateOfBirth.AddHours(1)};
        }

        protected override void DeleteAll()
        {
            var collections = _db.ListCollections();
            foreach (var collection in collections.ToList())
            {
                var name = collection.GetElement("name").Value.AsString;
                _db.DropCollection(name);
            }
        }
    }
}