namespace LogicMine.DataObject.GetObject
{
    public class GetObjectResponse<T> : Response
    {
        public T Object { get; }

        public GetObjectResponse()
        {
        }
        
        public GetObjectResponse(string error) : base(error)
        {
        }
        
        public GetObjectResponse(T obj)
        {
            Object = obj;
        }
    }
}