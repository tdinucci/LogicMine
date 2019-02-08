using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine.DataObject.UpdateObject;
using Newtonsoft.Json.Linq;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceUpdateObjectTerminal<T> : Terminal<UpdateObjectRequest<T, string>, UpdateObjectResponse>
    {
        private readonly IForceClient _forceClient;
        private readonly SalesforceObjectDescriptor<T> _descriptor;

        public SalesforceUpdateObjectTerminal(IForceClient forceClient, SalesforceObjectDescriptor<T> descriptor)
        {
            _forceClient = forceClient ?? throw new ArgumentNullException(nameof(forceClient));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public override async Task AddResponseAsync(
            IBasket<UpdateObjectRequest<T, string>, UpdateObjectResponse> basket)
        {
            var request = basket.Payload.Request;
            if (request == null)
                throw new InvalidOperationException("Basket does not contain a delta");

            if (string.IsNullOrWhiteSpace(request.Id))
                throw new InvalidOperationException("The delta contains no id");

            if (request.ModifiedProperties == null || !request.ModifiedProperties.Any())
                throw new InvalidOperationException("The delta contains no properties");

            var jobj = new JObject();
            foreach (var modifiedProperty in request.ModifiedProperties)
            {
                var mappedPropertyName = _descriptor.GetMappedColumnName(modifiedProperty.Key);
                jobj.Add(mappedPropertyName, JToken.FromObject(modifiedProperty.Value));
            }

            var sfResponse = await _forceClient.UpdateAsync(_descriptor.SalesforceTypeName, request.Id, jobj)
                .ConfigureAwait(false);

            if (!sfResponse.Success)
                throw new InvalidOperationException($"Patch failed: {sfResponse.Errors}");

            basket.Payload.Response = new UpdateObjectResponse(true);
        }
    }
}