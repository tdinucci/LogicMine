using System;
using System.Text;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.Salesforce;
using Sample.LogicMine.Web.Mine.MyContact;

namespace Sample.LogicMine.Web.Mine
{
    public static class MineFactory
    {
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";

        private static readonly Type[] StandardDataTypes =
        {
            typeof(MyContact.MyContact)
        };

        public static IMine Create()
        {
            return Generate<MyContact.MyContact>(new MyTraceExporter(), GetDescriptorRegistry());
        }

        private static IMine Generate<T>(ITraceExporter traceExporter, IDataObjectDescriptorRegistry descriptorRegistry)
            where T : new()
        {
            var forceClient = ForceClientFactory.CreateAsync(SfClientId, SfClientSecret, SfUsername, SfPassword, false)
                .GetAwaiter().GetResult();

            return new global::LogicMine.Mine()
                .AddShaft(new Shaft<GetObjectRequest<T, string>, GetObjectResponse<T>>(traceExporter,
                    new SalesforceGetObjectTerminal<T>(forceClient,
                        descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>()),
                    new SecurityStation()));
        }
        
        private static IDataObjectDescriptorRegistry GetDescriptorRegistry()
        {
            return new DataObjectDescriptorRegistry()
                .Register(new MyContactObjectDescriptor());
        }
        
        private class MyTraceExporter : ITraceExporter
        {
            public string Trace { get; private set; }
            public string Error { get; private set; }

            public void Export(IBasket basket)
            {
                var sb = new StringBuilder();
                foreach (var visit in basket.Visits)
                    sb.AppendLine($"{visit.Description} {visit.Direction}");

                Trace = sb.ToString();
            }

            public void ExportError(Exception exception)
            {
                Error = exception.Message;
            }

            public void ExportError(string error)
            {
                Error = error;
            }
        }
    }
}