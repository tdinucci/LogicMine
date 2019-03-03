namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectBasket<T, TId> : Basket<UpdateObjectRequest<T, TId>, UpdateObjectResponse>
    {
        public UpdateObjectBasket(UpdateObjectRequest<T, TId> request) : base(request)
        {
        }
    }
}