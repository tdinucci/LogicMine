using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.GetCollection
{
    /// <summary>
    /// A terminal which reads a collection of objects from an IDataObjectStore
    /// </summary>
    /// <typeparam name="T">The type read</typeparam>
    public class GetCollectionTerminal<T> : Terminal<GetCollectionRequest<T>, GetCollectionResponse<T>>
    {
        private readonly IDataObjectStore<T> _dataObjectStore;

        /// <summary>
        /// Construct a GetCollectionTerminal
        /// </summary>
        /// <param name="dataObjectStore">The data store</param>
        public GetCollectionTerminal(IDataObjectStore<T> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Attempts to get the T's from the datastore and populates the basket response
        /// </summary>
        /// <param name="basket">The basket which contains the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket argument is null</exception>
        public override async Task AddResponseAsync(IBasket<GetCollectionRequest<T>, GetCollectionResponse<T>> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            // just let any exceptions bubble up so they they can be handled by the Shaft
            var request = basket.Request;
            T[] collection;

            if (request.Filter != null)
            {
                collection = await _dataObjectStore
                    .GetCollectionAsync(request.Filter, request.Max, request.Page, request.Select)
                    .ConfigureAwait(false);
            }
            else
            {
                collection = await _dataObjectStore.GetCollectionAsync(request.Max, request.Page, request.Select)
                    .ConfigureAwait(false);
            }

            basket.Response = new GetCollectionResponse<T>(request, collection);
        }
    }
}