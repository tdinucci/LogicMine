namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectResponse<TId> : Response
    {
        public TId Id { get; }

        public CreateObjectResponse(TId id)
        {
            Id = id;
        }

        public CreateObjectResponse(string error) : base(error)
        {
        }
    }
}