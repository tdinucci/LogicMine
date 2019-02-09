using System;

namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionResponse<T> : Response
    {
        public string ObjectType { get; }
        public T[] Objects { get; }

        public GetCollectionResponse()
        {
        }

        public GetCollectionResponse(Guid requestId, T[] objects) : base(requestId)
        {
            ObjectType = typeof(T).Name;
            Objects = objects;
        }
    }
}