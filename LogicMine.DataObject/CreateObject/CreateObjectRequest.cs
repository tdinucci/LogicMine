using System;

namespace LogicMine.DataObject.CreateObject
{
    /// <summary>
    /// A request to create a data object
    /// </summary>
    /// <typeparam name="T">The type to create</typeparam>
    public class CreateObjectRequest<T> : Request
        where T : class
    {
        /// <summary>
        /// The type of object to create
        /// </summary>
        public Type ObjectType { get; } = typeof(T);
        
        /// <summary>
        /// The object to create
        /// </summary>
        public T Object { get; }

        /// <summary>
        /// Construct a new CreateObjectRequest
        /// </summary>
        /// <param name="obj">The object to create</param>
        public CreateObjectRequest(T obj)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));
        }
    }
}