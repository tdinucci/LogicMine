namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectResponse<TId> : Response
    {
        public TId ObjectId { get; }

        public CreateObjectResponse(IRequest request) : base(request)
        {
        }

        public CreateObjectResponse(IRequest request, TId objectId) : base(request)
        {
            ObjectId = objectId;
        }
    }
}