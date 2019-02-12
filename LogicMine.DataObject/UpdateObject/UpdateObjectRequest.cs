using System;
using System.Collections.Generic;

namespace LogicMine.DataObject.UpdateObject
{
    /// <summary>
    /// A request to update a T
    /// </summary>
    /// <typeparam name="T">The type to update</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class UpdateObjectRequest<T, TId> : Request
    {
        /// <summary>
        /// The type to update
        /// </summary>
        public Type ObjectType { get; } = typeof(T);
        
        /// <summary>
        /// The identity of the object to update
        /// </summary>
        public TId ObjectId { get; }
        
        /// <summary>
        /// The properties to modify 
        /// </summary>
        public IDictionary<string, object> ModifiedProperties { get; }

        /// <summary>
        /// Construct a UpdateObjectRequest
        /// </summary>
        /// <param name="objectId">The identity of the object to update</param>
        /// <param name="modifiedProperties">The properties on the object to update</param>
        /// <exception cref="ArgumentException">Thrown if the modifiedProperties argument is null or empty</exception>
        public UpdateObjectRequest(TId objectId, IDictionary<string, object> modifiedProperties)
        {
            if (modifiedProperties == null || modifiedProperties.Count == 0)
                throw new ArgumentException($"'{nameof(modifiedProperties)}' must have a value");

            ObjectId = objectId;
            ModifiedProperties = modifiedProperties;
        }
    }
}