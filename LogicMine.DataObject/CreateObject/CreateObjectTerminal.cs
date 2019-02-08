using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectTerminal<T, TId> : Terminal<CreateObjectRequest<T>, CreateObjectResponse<TId>>
        where T : class
    {
        private readonly IObjectStore<T, TId> _objectStore;

        public CreateObjectTerminal(IObjectStore<T, TId> objectStore)
        {
            _objectStore = objectStore ?? throw new ArgumentNullException(nameof(objectStore));
        }

        public override async Task AddResponseAsync(IBasket<CreateObjectRequest<T>, CreateObjectResponse<TId>> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            var id = await _objectStore.CreateAsync(basket.Payload.Request.Object).ConfigureAwait(false);
            basket.Payload.Response = new CreateObjectResponse<TId>(id);
        }
    }
}