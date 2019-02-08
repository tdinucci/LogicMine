using System;

namespace LogicMine.DataObject.GetObject
{
    public class GetObjectRequest<T, TId> : Request
    {
        public Type DataType { get; } = typeof(T);
        public TId Id { get; }

        public GetObjectRequest(TId id) 
        {
            Id = id;
        }
    }
}