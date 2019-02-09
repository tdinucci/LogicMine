using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine
{
    public static class MineFactory
    {
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";
        private const string SfAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";

        private static readonly SalesforceConnectionConfig SfConfig;

        static MineFactory()
        {
            SfConfig =
                new SalesforceConnectionConfig(SfClientId, SfClientSecret, SfUsername, SfPassword, SfAuthEndpoint);
        }

        public static IMine Create(IDataObjectDescriptorRegistry descriptorRegistry)
        {
            return Generate<MyContact.MyContact>(new DefaultTraceExporter(), descriptorRegistry);
        }

        private static IMine Generate<T>(ITraceExporter traceExporter, IDataObjectDescriptorRegistry descriptorRegistry)
            where T : new()
        {
            var mine = new global::LogicMine.Mine();
            AddStandardDataShafts<T>(mine, traceExporter, descriptorRegistry);

            return mine;
        }

        private static void AddStandardDataShafts<T>(IMine mine, ITraceExporter traceExporter,
            IDataObjectDescriptorRegistry descriptorRegistry)
            where T : new()
        {
            var descriptor = descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>();
            var objectStore = new SalesforceObjectStore<T>(SfConfig, descriptor);

            mine
                .AddShaft(new Shaft<GetObjectRequest<T, string>, GetObjectResponse<T>>(traceExporter,
                    new GetObjectTerminal<T, string>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<GetCollectionRequest<T>, GetCollectionResponse<T>>(traceExporter,
                    new GetCollectionTerminal<T>(objectStore),
                    new SecurityStation()));
        }
    }
}