using System;
using System.Net.Http;
using Dinucci.Salesforce.Client.Auth;
using Dinucci.Salesforce.Client.Data;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Salesforce.Util
{
    public class DataGenerator
    {
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";
        private const string SfAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";

        private readonly HttpClient _httpClient;

        public DataGenerator(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public IDataObjectStore<Frog<string>, string> GetStore()
        {
            var sfAuthenticator = new PasswordFlowAuthenticator(SfClientId, SfClientSecret, SfUsername, SfPassword,
                SfAuthEndpoint, _httpClient);

            var sfDataApi = new DataApi(sfAuthenticator, _httpClient, 44);

            return new FrogObjectStore(sfDataApi);
        }

        public void DeleteAll()
        {
            var store = GetStore();
            var collection = store.GetCollectionAsync().GetAwaiter().GetResult();

            foreach (var frog in collection)
                store.DeleteAsync(frog.Id).GetAwaiter().GetResult();
        }
    }
}
