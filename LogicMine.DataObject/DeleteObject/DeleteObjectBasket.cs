namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectBasket<T, TId> : Basket<DeleteObjectRequest<T, TId>, DeleteObjectResponse<T, TId>>
    {
        public DeleteObjectBasket(DeleteObjectRequest<T, TId> request) : base(request)
        {
        }
    }
}