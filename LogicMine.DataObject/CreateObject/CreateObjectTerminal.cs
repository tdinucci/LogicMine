using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectTerminal<T, TId> : Terminal<CreateObjectRequest<T>, CreateObjectResponse<T, TId>>
        where T : class
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        public CreateObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        public override async Task AddResponseAsync(
            IBasket<CreateObjectRequest<T>, CreateObjectResponse<T, TId>> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var objectId = await _dataObjectStore.CreateAsync(basket.Request.Object).ConfigureAwait(false);
            basket.Response = new CreateObjectResponse<T, TId>(basket.Request, objectId);
        }
    }
}