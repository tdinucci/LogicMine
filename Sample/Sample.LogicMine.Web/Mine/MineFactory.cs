using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.Salesforce;
using LogicMine.DataObject.UpdateObject;
using Sample.LogicMine.Web.Mine.GetTime;

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

        public static IMine Create(IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter)
        {
            return new global::LogicMine.Mine()
                .AddStandardDataShafts<MyContact.MyContact>(traceExporter, descriptorRegistry)

                .AddShaft(GetBasicShaft(traceExporter, new GetTimeTerminal()));
        }

        private static IMine AddStandardDataShafts<T>(this IMine mine, ITraceExporter traceExporter,
            IDataObjectDescriptorRegistry descriptorRegistry)
            where T : class, new()
        {
            var descriptor = descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>();
            var objectStore = new SalesforceObjectStore<T>(SfConfig, descriptor);

            return mine
                .AddShaft(GetBasicShaft(traceExporter, new GetObjectTerminal<T, string>(objectStore)))
                .AddShaft(GetBasicShaft(traceExporter, new GetCollectionTerminal<T>(objectStore)))
                .AddShaft(GetBasicShaft(traceExporter, new CreateObjectTerminal<T, string>(objectStore)))
                .AddShaft(GetBasicShaft(traceExporter, new UpdateObjectTerminal<T, string>(objectStore)))
                .AddShaft(GetBasicShaft(traceExporter, new DeleteObjectTerminal<T, string>(objectStore)));
        }

        private static IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(ITraceExporter traceExporter,
            ITerminal<TRequest, TResponse> terminal)
            where TRequest : class, IRequest
            where TResponse : IResponse, new()
        {
            return new Shaft<TRequest, TResponse>(traceExporter, terminal, new SecurityStation());
        }
    }
}