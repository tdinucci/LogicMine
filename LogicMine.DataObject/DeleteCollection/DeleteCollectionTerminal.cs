using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.DeleteCollection
{
    /// <summary>
    /// A terminal which deletes a collection of objects from an IDataObjectStore
    /// </summary>
    /// <typeparam name="T">The type read</typeparam>
    public class DeleteCollectionTerminal<T> : Terminal<DeleteCollectionRequest<T>, DeleteCollectionResponse>
    {
        private readonly IDataObjectStore<T> _dataObjectStore;

        /// <summary>
        /// Construct a DeleteCollectionTerminal
        /// </summary>
        /// <param name="dataObjectStore">The data store</param>
        public DeleteCollectionTerminal(IDataObjectStore<T> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Attempts to delete the T's from the datastore
        /// </summary>
        /// <param name="basket">The basket which contains the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket argument is null</exception>
        public override async Task AddResponseAsync(
            IBasket<DeleteCollectionRequest<T>, DeleteCollectionResponse> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            // just let any exceptions bubble up so they they can be handled by the Shaft
            var request = basket.Request;

            // this is just a safety precaution to save a bug in someone's code from wiping out a table.
            // you can always specify a filter that would delete all, e.g. Id > 0
            if (request.Filter == null)
                throw new InvalidOperationException("A filter is required");

            await _dataObjectStore.DeleteCollectionAsync(request.Filter).ConfigureAwait(false);
            basket.Response = new DeleteCollectionResponse(request, true);
        }
    }
}