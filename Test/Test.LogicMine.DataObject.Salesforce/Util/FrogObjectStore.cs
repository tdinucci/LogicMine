using LogicMine.DataObject.Salesforce;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Salesforce.Util
{
    public class FrogObjectStore : SalesforceObjectStore<Frog<string>>
    {
        private static readonly FrogDescriptor ObjDescriptor = new FrogDescriptor();

        public FrogObjectStore(SalesforceCredentials credentials) : base(credentials, ObjDescriptor)
        {
        }
    }
}