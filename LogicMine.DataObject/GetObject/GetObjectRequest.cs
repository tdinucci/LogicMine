using System;

namespace LogicMine.DataObject.GetObject
{
    public class GetObjectRequest<T, TId> : Request
    {
        public Type ObjectType { get; } = typeof(T);
        public TId ObjectId { get; }

        public GetObjectRequest(TId objectId) 
        {
            ObjectId = objectId;
        }
    }
}