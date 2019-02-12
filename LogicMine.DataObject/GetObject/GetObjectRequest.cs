using System;

namespace LogicMine.DataObject.GetObject
{
    /// <summary>
    /// A request to get an object by id
    /// </summary>
    /// <typeparam name="T">The type required</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class GetObjectRequest<T, TId> : Request
    {
        /// <summary>
        /// The required data type
        /// </summary>
        public Type ObjectType { get; } = typeof(T);

        /// <summary>
        /// The identity of the object required
        /// </summary>
        public TId ObjectId { get; }

        /// <summary>
        /// Construct a GetObjectRequest
        /// </summary>
        /// <param name="objectId">The identity of the object required</param>
        public GetObjectRequest(TId objectId)
        {
            ObjectId = objectId;
        }
    }
}