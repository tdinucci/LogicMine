using System;

namespace LogicMine.DataObject.Salesforce
{
    public abstract class SalesforceDataObjectShaftRegistrar<T> : DataObjectShaftRegistrar<T, string>
        where T : class, new()
    {
        protected SalesforceConnectionConfig SalesforceConnectionConfig { get; }
        protected SalesforceObjectDescriptor<T> Descriptor { get; }

        protected SalesforceDataObjectShaftRegistrar(SalesforceConnectionConfig connectionConfig,
            IDataObjectDescriptorRegistry descriptorRegistry)
        {
            if (descriptorRegistry == null) throw new ArgumentNullException(nameof(descriptorRegistry));

            SalesforceConnectionConfig = connectionConfig ?? throw new ArgumentNullException(nameof(connectionConfig));
            Descriptor = descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>();
        }

        protected override IDataObjectStore<T, string> GetDataObjectStore()
        {
            return new SalesforceObjectStore<T>(SalesforceConnectionConfig, Descriptor);
        }
    }
}