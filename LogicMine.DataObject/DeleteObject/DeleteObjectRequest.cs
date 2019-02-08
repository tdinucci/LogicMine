using System;

namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectRequest<T, TId> : Request
    {
        public Type DataType { get; } = typeof(T);
        public TId Id { get; }

        public DeleteObjectRequest(TId id)
        {
            Id = id;
        }
    }
}