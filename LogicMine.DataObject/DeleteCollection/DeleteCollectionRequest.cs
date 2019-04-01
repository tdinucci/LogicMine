using System;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject.DeleteCollection
{
    /// <summary>
    /// A request to delete a collection of data objects
    /// </summary>
    /// <typeparam name="T">The data type to delete</typeparam>
    public class DeleteCollectionRequest<T> : Request
    {
        /// <summary>
        /// The data type to delete
        /// </summary>
        public Type ObjectType { get; } = typeof(T);

        /// <summary>
        /// A filter to apply to the set of T
        /// </summary>
        public IFilter<T> Filter { get; }

        /// <summary>
        /// Constructs a DeleteCollectionRequest
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        public DeleteCollectionRequest(IFilter<T> filter)
        {
            Filter = filter;
        }
    }
}