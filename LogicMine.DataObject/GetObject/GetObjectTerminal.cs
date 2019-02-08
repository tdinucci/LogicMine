using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.GetObject
{
    public class GetObjectTerminal<T, TId> : Terminal<GetObjectRequest<T, TId>, GetObjectResponse<T>>
    {
        private readonly IObjectStore<T, TId> _objectStore;

        public GetObjectTerminal(IObjectStore<T, TId> objectStore)
        {
            _objectStore = objectStore ?? throw new ArgumentNullException(nameof(objectStore));
        }

        public override async Task AddResponseAsync(IBasket<GetObjectRequest<T, TId>, GetObjectResponse<T>> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var obj = await _objectStore.GetByIdAsync(basket.Payload.Request.Id).ConfigureAwait(false);
            basket.Payload.Response = new GetObjectResponse<T>(obj);
        }
    }
}