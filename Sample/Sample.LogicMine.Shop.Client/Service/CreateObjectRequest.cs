using System;

namespace Sample.LogicMine.Shop.Client.Service
{
    public class CreateObjectRequest<T> : ObjectRequest<T>
        where T : class
    {   
        public T Object { get; }

        public CreateObjectRequest(T obj)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));
        }
    }
}