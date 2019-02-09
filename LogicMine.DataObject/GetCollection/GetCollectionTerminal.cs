using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionTerminal<T> : Terminal<GetCollectionRequest<T>, GetCollectionResponse<T>>
    {
        private readonly IDataObjectStore<T> _dataObjectStore;

        public GetCollectionTerminal(IDataObjectStore<T> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        public override async Task AddResponseAsync(IBasket<GetCollectionRequest<T>, GetCollectionResponse<T>> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var request = basket.Payload.Request;
            T[] collection;

            if (request.Filter != null)
            {
                collection = await _dataObjectStore.GetCollectionAsync(request.Filter, request.Max, request.Page)
                    .ConfigureAwait(false);
            }
            else
            {
                collection = await _dataObjectStore.GetCollectionAsync(request.Max, request.Page)
                    .ConfigureAwait(false);
            }

            basket.Payload.Response = new GetCollectionResponse<T>(collection);
        }
    }
}