using System;

namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionResponse<T> : Response
    {
        public string ObjectType { get; }
        public T[] Objects { get; }

        public GetCollectionResponse(IRequest request) : base(request)
        {
        }

        public GetCollectionResponse(IRequest request, T[] objects) : base(request)
        {
            ObjectType = typeof(T).Name;
            Objects = objects;
        }
    }
}