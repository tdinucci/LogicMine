using System;

namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectRequest<T, TId> : Request
    {
        public Type ObjectType { get; } = typeof(T);
        public TId ObjectId { get; }

        public DeleteObjectRequest(TId objectId)
        {
            ObjectId = objectId;
        }
    }
}