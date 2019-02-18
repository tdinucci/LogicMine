using System;
using System.Net.Http;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.Salesforce.Util;

namespace Test.LogicMine.DataObject.Salesforce
{
    public class SalesforceMappedObjectStoreTest : MappedObjectStoreTest<Frog<string>, string>
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly DataGenerator _dataGenerator;

        public SalesforceMappedObjectStoreTest()
        {
            _dataGenerator = new DataGenerator(_httpClient);
        }

        protected override IDataObjectStore<Frog<string>, string> GetStore()
        {
            return _dataGenerator.GetStore();
        }

        protected override Frog<string> CreateFrog(int index, string name, DateTime dateOfBirth)
        {
            return new Frog<string> {Name = name, DateOfBirth = dateOfBirth};
        }

        protected override void DeleteAll()
        {
            _dataGenerator.DeleteAll();
        }
    }
}