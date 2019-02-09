using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine
{
    public class SampleDataObjectShaftRegistrar<T> : SalesforceDataObjectShaftRegistrar<T>
        where T : class, new()
    {
        protected ITraceExporter TraceExporter { get; }

        protected SampleDataObjectShaftRegistrar(SalesforceCredentials credentials,
            IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter) :
            base(credentials, descriptorRegistry)
        {
            TraceExporter = traceExporter;
        }

        protected override IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
        {
            return new Shaft<TRequest, TResponse>(TraceExporter, terminal, new SecurityStation());
        }
    }
}