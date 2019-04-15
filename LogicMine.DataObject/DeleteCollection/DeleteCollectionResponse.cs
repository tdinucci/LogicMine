namespace LogicMine.DataObject.DeleteCollection
{
    /// <summary>
    /// A response to an DeleteCollectionRequest
    /// </summary>
    public class DeleteCollectionResponse<T> : Response<DeleteCollectionRequest<T>>
    {
        /// <summary>
        /// True if the operation was successful
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Construct a DeleteCollectionResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public DeleteCollectionResponse(DeleteCollectionRequest<T> request) : base(request)
        {
        }

        /// <summary>
        /// Construct a DeleteCollectionResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="success">True if the operation was successful</param>
        /// <param name="error">Any error that occurred updating the object</param>
        public DeleteCollectionResponse(DeleteCollectionRequest<T> request, bool success, string error = null) :
            base(request, error)
        {
            Success = success;
        }
    }
}