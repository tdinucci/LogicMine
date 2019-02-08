namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectResponse : Response
    {
        public bool Success { get; }

        public DeleteObjectResponse(bool success, string error = null) : base(error)
        {
            Success = success;
        }
    }
}