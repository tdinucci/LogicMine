using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectTerminal<T, TId> : Terminal<DeleteObjectRequest<T, TId>, DeleteObjectResponse>
    {
        private readonly IObjectStore<T, TId> _objectStore;

        public DeleteObjectTerminal(IObjectStore<T, TId> objectStore)
        {
            _objectStore = objectStore ?? throw new ArgumentNullException(nameof(objectStore));
        }

        public override async Task AddResponseAsync(IBasket<DeleteObjectRequest<T, TId>, DeleteObjectResponse> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _objectStore.DeleteAsync(basket.Payload.Request.Id).ConfigureAwait(false);
            basket.Payload.Response = new DeleteObjectResponse(true);
        }
    }
}