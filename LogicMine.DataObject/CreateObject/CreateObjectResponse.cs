namespace LogicMine.DataObject.CreateObject
{
    /// <summary>
    /// A response to a CreateObjectRequest
    /// </summary>
    /// <typeparam name="T">The type that was created</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class CreateObjectResponse<T, TId> : Response where T : class
    {
        /// <summary>
        /// The id of the object that was created
        /// </summary>
        public TId ObjectId { get; }

        /// <summary>
        /// Construct a new CreateObjectResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public CreateObjectResponse(IRequest request) : base(request)
        {
        }

        /// <summary>
        /// Construct a new CreateObjectResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="objectId">The id of the object that was created</param>
        public CreateObjectResponse(IRequest request, TId objectId) : base(request)
        {
            ObjectId = objectId;
        }
    }
}