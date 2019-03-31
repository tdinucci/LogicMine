using System;
using System.Collections.Generic;

namespace LogicMine.DataObject.CreateCollection
{
    /// <summary>
    /// A request to create a collection of data objects
    /// </summary>
    /// <typeparam name="T">The type to create</typeparam>
    public class CreateCollectionRequest<T> : Request
        where T : class
    {
        /// <summary>
        /// The type of objects to create
        /// </summary>
        public Type ObjectType { get; } = typeof(T);

        /// <summary>
        /// The objects to create
        /// </summary>
        public IEnumerable<T> Objects { get; }

        /// <summary>
        /// Construct a new CreateObjectCollectionRequest
        /// </summary>
        /// <param name="objs">The objects to create</param>
        public CreateCollectionRequest(IEnumerable<T> objs)
        {
            Objects = objs ?? throw new ArgumentNullException(nameof(objs));
        }
    }
}