namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectBasket<T, TId> : Basket<DeleteObjectRequest<T, TId>, DeleteObjectResponse>
    {
        public DeleteObjectBasket(DeleteObjectRequest<T, TId> request) : base(request)
        {
        }
    }
}