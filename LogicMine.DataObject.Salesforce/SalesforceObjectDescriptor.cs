using System;
using Newtonsoft.Json.Linq;

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

        /// <summary>
        /// Override if there is anything that needs to be done to an object before it's posted to Salesforce
        /// </summary>
        /// <param name="objToPost">The object which is about to be posted</param>
        public virtual void PrepareForPost(JObject objToPost)
        {
        }
    }
}