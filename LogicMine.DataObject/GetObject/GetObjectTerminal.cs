using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.GetObject
{
    public class GetObjectTerminal<T, TId> : Terminal<GetObjectRequest<T, TId>, GetObjectResponse<T>>
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        public GetObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        public override async Task AddResponseAsync(IBasket<GetObjectRequest<T, TId>, GetObjectResponse<T>> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var obj = await _dataObjectStore.GetByIdAsync(basket.Payload.Request.ObjectId).ConfigureAwait(false);
            basket.Payload.Response = new GetObjectResponse<T>(basket.Payload.Request.Id, obj);
        }
    }
}