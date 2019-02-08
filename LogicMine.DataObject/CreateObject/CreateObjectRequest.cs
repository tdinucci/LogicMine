using System;

namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectRequest<T> : Request
        where T : class
    {
        public T Object { get; }

        public CreateObjectRequest(T obj)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));
        }
    }
}