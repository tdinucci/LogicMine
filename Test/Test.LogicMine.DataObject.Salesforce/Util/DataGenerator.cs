using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Salesforce.Util
{
    public static class DataGenerator
    {
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";
        private const string SfAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";

        public static IDataObjectStore<Frog<string>, string> GetStore()
        {
            var sfConfig =
                new SalesforceCredentials(SfClientId, SfClientSecret, SfUsername, SfPassword, SfAuthEndpoint);

            return new FrogObjectStore(sfConfig);
        }

        public static void DeleteAll()
        {
            var store = GetStore();
            var collection = store.GetCollectionAsync().GetAwaiter().GetResult();

            foreach (var frog in collection)
                store.DeleteAsync(frog.Id).GetAwaiter().GetResult();
        }
    }
}