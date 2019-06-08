using System;
using LogicMine.DataObject.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.MongoDb.Util
{
    public class FrogDescriptor : MongoDbObjectDescriptor<Frog<Guid>>
    {
        static FrogDescriptor()
        {
            var conventionPack = new ConventionPack {new CamelCaseElementNameConvention()};
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            BsonClassMap.RegisterClassMap<Frog<Guid>>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
            });
        }

        public FrogDescriptor() : base("frog", nameof(Frog<Guid>.Id))
        {
        }
        
        public override string GetIdPropertyName()
        {
            return nameof(Frog<Guid>.Id);
        }

        public override Guid GetId(Frog<Guid> obj)
        {
            return obj.Id;
        }
    }
}