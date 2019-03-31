namespace LogicMine.DataObject.CreateCollection
{
    public class CreateCollectionBasket<T> :
        Basket<CreateCollectionRequest<T>, CreateCollectionResponse> where T : class
    {
        public CreateCollectionBasket(CreateCollectionRequest<T> request) : base(request)
        {
        }
    }
}