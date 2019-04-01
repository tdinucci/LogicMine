namespace LogicMine.DataObject.DeleteCollection
{
    public class DeleteCollectionBasket<T> : Basket<DeleteCollectionRequest<T>, DeleteCollectionResponse>
    {
        public DeleteCollectionBasket(DeleteCollectionRequest<T> request) : base(request)
        {
        }
    }
}