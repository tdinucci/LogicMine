namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectBasket<T, TId> : Basket<UpdateObjectRequest<T, TId>, UpdateObjectResponse<T, TId>>
    {
        public UpdateObjectBasket(UpdateObjectRequest<T, TId> request) : base(request)
        {
        }
    }
}