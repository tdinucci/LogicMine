using System;

namespace LogicMine.DataObject.Salesforce
{
    /// <summary>
    /// A basic descriptor for Salesforce types
    /// </summary>
    public class SalesforceObjectDescriptor<T> : DataObjectDescriptor<T, string>
    {
        public string SalesforceTypeName { get; }

        public SalesforceObjectDescriptor(string salesforceTypeName, params string[] readOnlyPropertyNames) :
            base(readOnlyPropertyNames)
        {
            if (string.IsNullOrWhiteSpace(salesforceTypeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(salesforceTypeName));

            SalesforceTypeName = salesforceTypeName;
            ReadOnlyPropertyNames.Add("Id");
        }

        protected bool IsPropertyNameMatch(string potentialPropertyName, string propertyName)
        {
            return string.Equals(potentialPropertyName, propertyName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}