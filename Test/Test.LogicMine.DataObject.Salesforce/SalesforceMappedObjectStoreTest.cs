using System;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.Salesforce.Util;

namespace Test.LogicMine.DataObject.Salesforce
{
    public class SalesforceMappedObjectStoreTest : MappedObjectStoreTest<Frog<string>, string>
    {
        protected override IDataObjectStore<Frog<string>, string> GetStore()
        {
            return DataGenerator.GetStore();
        }

        protected override Frog<string> CreateFrog(int index, string name, DateTime dateOfBirth)
        {
            return new Frog<string> {Name = name, DateOfBirth = dateOfBirth};
        }

        protected override void DeleteAll()
        {
            DataGenerator.DeleteAll();
        }
    }
}