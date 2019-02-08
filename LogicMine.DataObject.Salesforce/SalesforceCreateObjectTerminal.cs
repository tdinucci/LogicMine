using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine.DataObject.CreateObject;
using Newtonsoft.Json.Linq;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceCreateObjectTerminal<T> : Terminal<CreateObjectRequest<T>, CreateObjectResponse<string>>
        where T : class
    {
        private readonly IForceClient _forceClient;
        private readonly SalesforceObjectDescriptor<T> _descriptor;

        public SalesforceCreateObjectTerminal(IForceClient forceClient, SalesforceObjectDescriptor<T> descriptor)
        {
            _forceClient = forceClient ?? throw new ArgumentNullException(nameof(forceClient));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public override async Task AddResponseAsync(
            IBasket<CreateObjectRequest<T>, CreateObjectResponse<string>> basket)
        {
            var toCreate = basket.Payload.Request.Object;
            if (toCreate == null)
                throw new InvalidOperationException("There is no object to create in the basket");

            var properties = typeof(T).GetProperties()
                .Where(p => _descriptor.CanWrite(p.Name));

            var jobj = new JObject();
            foreach (var prop in properties)
            {
                var mappedProperty = _descriptor.GetMappedColumnName(prop.Name);
                var mappedPropertyValue = _descriptor.ProjectPropertyValue(prop.GetValue(toCreate), prop.Name);

                if (mappedPropertyValue != null)
                    jobj[mappedProperty] = JToken.FromObject(mappedPropertyValue);
            }

            _descriptor.PrepareForPost(jobj);

            var response = await _forceClient.CreateAsync(_descriptor.SalesforceTypeName, jobj).ConfigureAwait(false);
            if (!response.Success)
                throw new InvalidOperationException($"Failed to post object: {response.Errors}");

            basket.Payload.Response = new CreateObjectResponse<string>(response.Id);
        }
    }
}