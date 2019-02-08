using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionTerminal<T> : Terminal<GetCollectionRequest<T>, GetCollectionResponse<T>>
    {
        private readonly IObjectStore<T> _objectStore;

        public GetCollectionTerminal(IObjectStore<T> objectStore)
        {
            _objectStore = objectStore ?? throw new ArgumentNullException(nameof(objectStore));
        }

        public override async Task AddResponseAsync(IBasket<GetCollectionRequest<T>, GetCollectionResponse<T>> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var request = basket.Payload.Request;
            T[] collection;

            if (request.Filter != null)
            {
                collection = await _objectStore.GetCollectionAsync(request.Filter, request.Max, request.Page)
                    .ConfigureAwait(false);
            }
            else
            {
                collection = await _objectStore.GetCollectionAsync(request.Max, request.Page)
                    .ConfigureAwait(false);
            }

            basket.Payload.Response = new GetCollectionResponse<T>(collection);
        }
    }
}