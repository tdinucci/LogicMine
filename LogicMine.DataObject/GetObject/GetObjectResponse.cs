namespace LogicMine.DataObject.GetObject
{
    public class GetObjectResponse<T> : Response
    {
        public string ObjectType { get; }
        public T Object { get; }

        public GetObjectResponse(IRequest request) : base(request)
        {
        }

        public GetObjectResponse(IRequest request, T obj) : base(request)
        {
            ObjectType = typeof(T).Name;
            Object = obj;
        }
    }
}