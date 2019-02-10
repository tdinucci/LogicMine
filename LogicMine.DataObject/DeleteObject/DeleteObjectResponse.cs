namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectResponse : Response
    {
        public bool Success { get; }

        public DeleteObjectResponse(IRequest request) : base(request)
        {
        }

        public DeleteObjectResponse(IRequest request, bool success, string error = null) : base(request, error)
        {
            Success = success;
        }
    }
}