using System;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.Common.LogicMine.Mine;
using Test.LogicMine.DataObject.Salesforce.Util;

namespace Test.LogicMine.DataObject.Salesforce
{
    public class SalesforceMineTest : MineTest<Frog<string>, string>
    {
        protected override IDataObjectDescriptor GetDescriptor()
        {
            return new FrogDescriptor();
        }

        protected override IDataObjectStore<Frog<string>, string> GetObjectStore()
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