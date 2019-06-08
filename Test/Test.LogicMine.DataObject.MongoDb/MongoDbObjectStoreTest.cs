using System;
using LogicMine.DataObject;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Test.Common.LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.MongoDb.Util;

namespace Test.LogicMine.DataObject.MongoDb
{
    public class MongoDbObjectStoreTest : MappedObjectStoreTest<Frog<Guid>, Guid>
    {
        private readonly IMongoDatabase _db;

        public MongoDbObjectStoreTest()
        {
            _db = new MongoClient("mongodb://localhost:27017").GetDatabase("logicMineTest");
        }

        protected override IDataObjectStore<Frog<Guid>, Guid> GetStore()
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