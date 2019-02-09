using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Manager
{
    public class ManagerShaftRegistrar : SampleDataObjectShaftRegistrar<Manager>
    {
        public ManagerShaftRegistrar(SalesforceConnectionConfig connectionConfig,
            IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter) :
            base(connectionConfig, descriptorRegistry, traceExporter)
        {
        }

        protected override IDataObjectStore<Manager, string> GetDataObjectStore()
        {
            return new ManagerObjectStore(SalesforceConnectionConfig, Descriptor);
        }
    }
}