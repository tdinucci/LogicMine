namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionBasket<T> : Basket<GetCollectionRequest<T>, GetCollectionResponse<T>>
    {
        public GetCollectionBasket(GetCollectionRequest<T> request) : base(request)
        {
        }
    }
}