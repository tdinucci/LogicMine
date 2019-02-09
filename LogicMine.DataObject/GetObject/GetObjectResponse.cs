using System;

namespace LogicMine.DataObject.GetObject
{
    public class GetObjectResponse<T> : Response
    {
        public string ObjectType { get; }
        public T Object { get; }

        public GetObjectResponse()
        {
        }

        public GetObjectResponse(Guid requestId, T obj) : base(requestId)
        {
            ObjectType = typeof(T).Name;
            Object = obj;
        }
    }
}