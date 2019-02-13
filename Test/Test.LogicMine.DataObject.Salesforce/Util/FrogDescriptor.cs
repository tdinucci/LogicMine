using System;
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
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));

            propertyName = propertyName.ToLower();
            if (propertyName == nameof(Frog<string>.Id).ToLower())
                return "Id";
            if (propertyName == nameof(Frog<string>.Name).ToLower())
                return "Name__c";
            if (propertyName == nameof(Frog<string>.DateOfBirth).ToLower())
                return "DateOfBirth__c";

            return base.GetMappedColumnName(propertyName);
        }
    }
}