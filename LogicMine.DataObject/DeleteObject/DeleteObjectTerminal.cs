using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.DeleteObject
{
    /// <summary>
    /// A terminal which deletes a T from an IDataObjectStore
    /// </summary>
    /// <typeparam name="T">The type to delete</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class DeleteObjectTerminal<T, TId> : Terminal<DeleteObjectRequest<T, TId>, DeleteObjectResponse<T, TId>>
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        /// <summary>
        /// Construct a DeleteObjectTerminal
        /// </summary>
        /// <param name="dataObjectStore">The data source</param>
        public DeleteObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Deletes a T from the datastore and populates the baskets response
        /// </summary>
        /// <param name="basket">The basket which contains the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket argument is null</exception>
        public override async Task AddResponseAsync(
            IBasket<DeleteObjectRequest<T, TId>, DeleteObjectResponse<T, TId>> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _dataObjectStore.DeleteAsync(basket.Request.ObjectId).ConfigureAwait(false);
            basket.Response = new DeleteObjectResponse<T, TId>(basket.Request, true);
        }
    }
}