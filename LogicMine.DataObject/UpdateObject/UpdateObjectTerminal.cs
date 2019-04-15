using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.UpdateObject
{
    /// <summary>
    /// A terminal which updates an object in an IDataObjectStore
    /// </summary>
    /// <typeparam name="T">The type to update</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class UpdateObjectTerminal<T, TId> : Terminal<UpdateObjectRequest<T, TId>, UpdateObjectResponse<T, TId>>
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        /// <summary>
        /// Construct a UpdateObjectTerminal
        /// </summary>
        /// <param name="dataObjectStore">The datastore which contains the object to update</param>
        public UpdateObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Attempts to update the requested T and adds a response to the basket
        /// </summary>
        /// <param name="basket">The basket which contains the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket is null</exception>
        public override async Task AddResponseAsync(
            IBasket<UpdateObjectRequest<T, TId>, UpdateObjectResponse<T, TId>> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _dataObjectStore
                .UpdateAsync(basket.Request.ObjectId, basket.Request.ModifiedProperties)
                .ConfigureAwait(false);

            basket.Response = new UpdateObjectResponse<T, TId>(basket.Request, true);
        }
    }
}