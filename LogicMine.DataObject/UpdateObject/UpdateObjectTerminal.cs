using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectTerminal<T, TId> : Terminal<UpdateObjectRequest<T, TId>, UpdateObjectResponse>
    {
        private readonly IObjectStore<T, TId> _objectStore;

        public UpdateObjectTerminal(IObjectStore<T, TId> objectStore)
        {
            _objectStore = objectStore ?? throw new ArgumentNullException(nameof(objectStore));
        }

        public override async Task AddResponseAsync(IBasket<UpdateObjectRequest<T, TId>, UpdateObjectResponse> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _objectStore.UpdateAsync(basket.Payload.Request.Id, basket.Payload.Request.ModifiedProperties)
                .ConfigureAwait(false);

            basket.Payload.Response = new UpdateObjectResponse(true);
        }
    }
}