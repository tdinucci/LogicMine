using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.CreateCollection
{
    /// <summary>
    /// A terminal which creates a collection of T within an IDataObjectStore
    /// </summary>
    /// <typeparam name="T">The type to create</typeparam>
    public class CreateCollectionTerminal<T> :
        Terminal<CreateCollectionRequest<T>, CreateCollectionResponse> where T : class
    {
        private readonly IDataObjectStore<T> _dataObjectStore;

        /// <summary>
        /// Constructs a CreateObjectCollectionTerminal
        /// </summary>
        /// <param name="dataObjectStore">The datastore to create the T's in</param>
        public CreateCollectionTerminal(IDataObjectStore<T> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        /// <summary>
        /// Creates the requested T's in the datastore
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the basket argument is null</exception>
        public override async Task AddResponseAsync(
            IBasket<CreateCollectionRequest<T>, CreateCollectionResponse> basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _dataObjectStore.CreateCollectionAsync(basket.Request.Objects).ConfigureAwait(false);
            basket.Response = new CreateCollectionResponse(basket.Request, true);
        }
    }
}