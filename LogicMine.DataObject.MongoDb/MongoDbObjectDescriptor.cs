using System;

namespace LogicMine.DataObject.MongoDb
{
    /// <summary>
    /// A type which contains metadata related to objects which are stored in a MongoDb database.
    /// </summary>
    public abstract class MongoDbObjectDescriptor<T> : DataObjectDescriptor<T, Guid>
    {
        public string CollectionName { get; }

        /// <summary>
        /// Construct a new MongoDbObjectDescriptor
        /// </summary>
        /// <param name="collectionName">The MongoDb collection that the type resides in</param>
        /// <param name="readOnlyPropertyNames">Properties on the type that are read only</param>
        /// <exception cref="ArgumentException"></exception>
        protected MongoDbObjectDescriptor(string collectionName, params string[] readOnlyPropertyNames) :
            base(readOnlyPropertyNames)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(collectionName));

            CollectionName = collectionName;
        }

        /// <summary>
        /// Get the name of the identity property on the described type
        /// </summary>
        /// <returns></returns>
        public abstract string GetIdPropertyName();

        /// <summary>
        /// Get the identity value from the provided object
        /// </summary>
        /// <param name="obj">The object to get the identity from</param>
        /// <returns>The identity of 'obj'</returns>
        public abstract Guid GetId(T obj);
    }
}