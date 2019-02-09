using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Manager
{
    public class ManagerObjectStore : SalesforceContactObjectStore<Manager>
    {
        protected override string SalesforceContactType { get; } = "Manager";

        public ManagerObjectStore(SalesforceConnectionConfig connectionConfig,
            SalesforceObjectDescriptor<Manager> descriptor) : base(connectionConfig, descriptor)
        {
        }
    }
}