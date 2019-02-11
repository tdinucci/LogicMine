using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectTerminal<T, TId> : Terminal<DeleteObjectRequest<T, TId>, DeleteObjectResponse>
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        public DeleteObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        public override async Task AddResponseAsync(IBasket<DeleteObjectRequest<T, TId>, DeleteObjectResponse> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _dataObjectStore.DeleteAsync(basket.Request.ObjectId).ConfigureAwait(false);
            basket.Response = new DeleteObjectResponse(basket.Request, true);
        }
    }
}