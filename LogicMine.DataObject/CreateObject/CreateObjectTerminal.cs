using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.CreateObject
{
    /// <summary>
    /// A terminal which creates a T within an IDataObjectStore
    /// </summary>
    /// <typeparam name="T">The type to create</typeparam>
    /// <typeparam name="TId">The identity type on the T</typeparam>
    public class CreateObjectTerminal<T, TId> : Terminal<CreateObjectRequest<T>, CreateObjectResponse<T, TId>>
        where T : class
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        /// <summary>
        /// Constructs a CreateObjectTerminal
        /// </summary>
        /// <param name="dataObjectStore">The datastore to create the T in</param>
        public CreateObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Creates the requested T in the datastore and populates the baskets response
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket argument is null</exception>
        public override async Task AddResponseAsync(
            IBasket<CreateObjectRequest<T>, CreateObjectResponse<T, TId>> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));
            
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var objectId = await _dataObjectStore.CreateAsync(basket.Request.Object).ConfigureAwait(false);
            basket.Response = new CreateObjectResponse<T, TId>(basket.Request, objectId);
        }
    }
}