namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectBasket<T, TId> : Basket<CreateObjectRequest<T>, CreateObjectResponse<T, TId>>
        where T : class
    {
        public CreateObjectBasket(CreateObjectRequest<T> request) : base(request)
        {
        }
    }
}