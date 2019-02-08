using System;
using System.Threading.Tasks;
using LogicMine.DataObject.DeleteObject;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceDeleteObjectTerminal<T> : Terminal<DeleteObjectRequest<T, string>, DeleteObjectResponse>
    {
        private readonly IForceClient _forceClient;
        private readonly SalesforceObjectDescriptor<T> _descriptor;

        public SalesforceDeleteObjectTerminal(IForceClient forceClient, SalesforceObjectDescriptor<T> descriptor)
        {
            _forceClient = forceClient ?? throw new ArgumentNullException(nameof(forceClient));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public override async Task AddResponseAsync(
            IBasket<DeleteObjectRequest<T, string>, DeleteObjectResponse> basket)
        {
            var id = basket.Payload?.Request?.Id;
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException("Basket does not contain an id");

            var isSuccess = await _forceClient.DeleteAsync(_descriptor.SalesforceTypeName, id).ConfigureAwait(false);

            if (!isSuccess)
                throw new InvalidOperationException("Failed to delete object");

            basket.Payload.Response = new DeleteObjectResponse(true);
        }
    }
}