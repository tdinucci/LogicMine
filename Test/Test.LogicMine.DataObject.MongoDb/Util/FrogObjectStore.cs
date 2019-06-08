using System;
using LogicMine.DataObject.MongoDb;
using MongoDB.Driver;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.MongoDb.Util
{
    public class FrogObjectStore : MongoDbObjectStore<Frog<Guid>>
    {
        public FrogObjectStore(IMongoDatabase mongoDb) : base(mongoDb, new FrogDescriptor())
        {
        }
    }
}