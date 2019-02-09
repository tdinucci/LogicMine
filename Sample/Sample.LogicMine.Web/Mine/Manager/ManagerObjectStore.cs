using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Manager
{
    public class ManagerObjectStore : SalesforceContactObjectStore<Manager>
    {
        protected override string SalesforceContactType { get; } = "Manager";

        public ManagerObjectStore(SalesforceCredentials credentials,
            SalesforceObjectDescriptor<Manager> descriptor) : base(credentials, descriptor)
        {
        }
    }
}