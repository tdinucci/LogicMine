namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectResponse : Response
    {
        public bool Success { get; }

        public UpdateObjectResponse(bool success, string error = null) : base(error)
        {
            Success = success;
        }
    }
}