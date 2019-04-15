namespace LogicMine.DataObject.DeleteCollection
{
    public class DeleteCollectionBasket<T> : Basket<DeleteCollectionRequest<T>, DeleteCollectionResponse<T>>
    {
        public DeleteCollectionBasket(DeleteCollectionRequest<T> request) : base(request)
        {
        }
    }
}