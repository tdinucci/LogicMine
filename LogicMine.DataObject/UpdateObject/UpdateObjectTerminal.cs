using System;
using System.Threading.Tasks;

namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectTerminal<T, TId> : Terminal<UpdateObjectRequest<T, TId>, UpdateObjectResponse>
    {
        private readonly IDataObjectStore<T, TId> _dataObjectStore;

        public UpdateObjectTerminal(IDataObjectStore<T, TId> dataObjectStore)
        {
            _dataObjectStore = dataObjectStore ?? throw new ArgumentNullException(nameof(dataObjectStore));
        }

        public override async Task AddResponseAsync(IBasket<UpdateObjectRequest<T, TId>, UpdateObjectResponse> basket)
        {
            // just let any exceptions bubble up so they they can be handled by the Shaft
            await _dataObjectStore
                .UpdateAsync(basket.Request.ObjectId, basket.Request.ModifiedProperties)
                .ConfigureAwait(false);

            basket.Response = new UpdateObjectResponse(basket.Request, true);
        }
    }
}