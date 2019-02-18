using System;
using Dinucci.Salesforce.Client.Data;

namespace LogicMine.DataObject.Salesforce
{
    public abstract class SalesforceDataObjectShaftRegistrar<T> : DataObjectShaftRegistrar<T, string>
        where T : class, new()
    {
        protected IDataApi SalesforceDataApi { get; }
        protected SalesforceObjectDescriptor<T> Descriptor { get; }

        protected SalesforceDataObjectShaftRegistrar(IDataApi salesforceDataApi,
            IDataObjectDescriptorRegistry descriptorRegistry)
        {
            if (descriptorRegistry == null) throw new ArgumentNullException(nameof(descriptorRegistry));
            SalesforceDataApi = salesforceDataApi ?? throw new ArgumentNullException(nameof(salesforceDataApi));

            Descriptor = descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>();
        }

        protected override IDataObjectStore<T, string> GetDataObjectStore()
        {
            return new SalesforceObjectStore<T>(SalesforceDataApi, Descriptor);
        }
    }
}