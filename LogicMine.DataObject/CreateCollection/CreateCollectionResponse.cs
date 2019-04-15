namespace LogicMine.DataObject.CreateCollection
{
    /// <summary>
    /// A response to an CreateObjectCollectionRequest
    /// </summary>
    public class CreateCollectionResponse<T> : Response<CreateCollectionRequest<T>> where T : class
    {
        /// <summary>
        /// True if the operation was successful
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Construct a CreateObjectCollectionResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public CreateCollectionResponse(CreateCollectionRequest<T> request) : base(request)
        {
        }

        /// <summary>
        /// Construct a CreateObjectCollectionResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="success">True if the operation was successful</param>
        /// <param name="error">Any error that occurred updating the object</param>
        public CreateCollectionResponse(CreateCollectionRequest<T> request, bool success, string error = null) :
            base(request, error)
        {
            Success = success;
        }
    }
}