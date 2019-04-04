using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.GetObject
{
    /// <summary>
    /// A terminal which retrieves a T by identity
    /// </summary>
    /// <typeparam name="T">The required type</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class GetObjectTerminal<T, TId> : Terminal<GetObjectRequest<T, TId>, GetObjectResponse<T>>
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        /// <summary>
        /// Construct a GetObjectTerminal
        /// </summary>
        /// <param name="dataObjectStore">The data store</param>
        public GetObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Attempts to retrieve the requested T from the datastore and populate the baskets response
        /// </summary>
        /// <param name="basket">The basket that contains the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket argument is null</exception>
        public override async Task AddResponseAsync(IBasket<GetObjectRequest<T, TId>, GetObjectResponse<T>> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));
            
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var obj = await _dataObjectStore.GetByIdAsync(basket.Request.ObjectId, basket.Request.Select).ConfigureAwait(false);
            basket.Response = new GetObjectResponse<T>(basket.Request, obj);
        }
    }
}