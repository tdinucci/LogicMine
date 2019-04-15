namespace LogicMine.DataObject.DeleteObject
{
    /// <summary>
    /// A response to a DeleteObjectReqest
    /// </summary>
    public class DeleteObjectResponse<T, TId> : Response<DeleteObjectRequest<T, TId>>
    {
        /// <summary>
        /// True if the deletion was successful
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Construct a DeleteObjectResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public DeleteObjectResponse(DeleteObjectRequest<T, TId> request) : base(request)
        {
        }

        /// <summary>
        /// Construct a DeleteObjectResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="success">Set to true if the deletion was successful</param>
        /// <param name="error">An error that occurred during the deletion</param>
        public DeleteObjectResponse(DeleteObjectRequest<T, TId> request, bool success, string error = null) :
            base(request, error)
        {
            Success = success;
        }
    }
}