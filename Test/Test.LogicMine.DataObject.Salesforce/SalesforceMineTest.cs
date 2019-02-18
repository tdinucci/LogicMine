using System;
using System.Net.Http;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.Common.LogicMine.Mine;
using Test.LogicMine.DataObject.Salesforce.Util;

namespace Test.LogicMine.DataObject.Salesforce
{
    public class SalesforceMineTest : MineTest<Frog<string>, string>, IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly DataGenerator _dataGenerator;

        public SalesforceMineTest()
        {
            _dataGenerator = new DataGenerator(_httpClient);
        }

        protected override IDataObjectDescriptor GetDescriptor()
        {
            return new FrogDescriptor();
        }

        protected override IDataObjectStore<Frog<string>, string> GetObjectStore()
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

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}