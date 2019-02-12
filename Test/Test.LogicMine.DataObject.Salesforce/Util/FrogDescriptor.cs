using LogicMine.DataObject.Salesforce;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Salesforce.Util
{
    public class FrogDescriptor : SalesforceObjectDescriptor<Frog<string>>
    {
        public FrogDescriptor() : base("Frog__c", "Id")
        {
        }

        public override string GetMappedColumnName(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Frog<string>.Id):
                    return "Id";
                case nameof(Frog<string>.Name):
                    return "Name__c";
                case nameof(Frog<string>.DateOfBirth):
                    return "DateOfBirth__c";
                default:
                    return base.GetMappedColumnName(propertyName);
            }
        }
    }
}