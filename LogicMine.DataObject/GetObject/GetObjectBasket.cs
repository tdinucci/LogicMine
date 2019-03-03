namespace LogicMine.DataObject.GetObject
{
    public class GetObjectBasket<T, TId> : Basket<GetObjectRequest<T, TId>, GetObjectResponse<T>>
    {
        public GetObjectBasket(GetObjectRequest<T, TId> request) : base(request)
        {
        }
    }
}