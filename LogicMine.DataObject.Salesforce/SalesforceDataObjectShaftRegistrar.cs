using System;

namespace LogicMine.DataObject.Salesforce
{
    public abstract class SalesforceDataObjectShaftRegistrar<T> : DataObjectShaftRegistrar<T, string>
        where T : class, new()
    {
        protected SalesforceCredentials SalesforceCredentials { get; }
        protected SalesforceObjectDescriptor<T> Descriptor { get; }

        protected SalesforceDataObjectShaftRegistrar(SalesforceCredentials credentials,
            IDataObjectDescriptorRegistry descriptorRegistry)
        {
            if (descriptorRegistry == null) throw new ArgumentNullException(nameof(descriptorRegistry));

            SalesforceCredentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Descriptor = descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>();
        }

        protected override IDataObjectStore<T, string> GetDataObjectStore()
        {
            return new SalesforceObjectStore<T>(SalesforceCredentials, Descriptor);
        }
    }
}