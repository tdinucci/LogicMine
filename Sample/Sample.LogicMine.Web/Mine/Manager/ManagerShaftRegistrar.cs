using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Manager
{
    public class ManagerShaftRegistrar : SampleDataObjectShaftRegistrar<Manager>
    {
        public ManagerShaftRegistrar(SalesforceCredentials credentials,
            IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter) :
            base(credentials, descriptorRegistry, traceExporter)
        {
        }

        protected override IDataObjectStore<Manager, string> GetDataObjectStore()
        {
            return new ManagerObjectStore(SalesforceCredentials, Descriptor);
        }
    }
}