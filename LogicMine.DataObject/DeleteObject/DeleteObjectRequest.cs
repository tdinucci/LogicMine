using System;

namespace LogicMine.DataObject.DeleteObject
{
    /// <summary>
    /// A request to delete a data object
    /// </summary>
    /// <typeparam name="T">The data type to delete</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class DeleteObjectRequest<T, TId> : Request
    {
        /// <summary>
        /// The data type to delete
        /// </summary>
        public Type ObjectType { get; } = typeof(T);

        /// <summary>
        /// The identity of the object to delete
        /// </summary>
        public TId ObjectId { get; }

        /// <summary>
        /// Constructs a DeleteObjectRequest
        /// </summary>
        /// <param name="objectId">The identity of the object to delete</param>
        public DeleteObjectRequest(TId objectId)
        {
            ObjectId = objectId;
        }
    }
}