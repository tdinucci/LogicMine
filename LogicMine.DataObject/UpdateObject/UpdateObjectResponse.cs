namespace LogicMine.DataObject.UpdateObject
{
    /// <summary>
    /// A response to an UpdateObjectRequest
    /// </summary>
    public class UpdateObjectResponse<T, TId> : Response<UpdateObjectRequest<T, TId>>
    {
        /// <summary>
        /// True if the operation was successful
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Construct a UpdateObjectResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public UpdateObjectResponse(UpdateObjectRequest<T, TId> request) : base(request)
        {
        }

        /// <summary>
        /// Construct a UpdateObjectResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="success">True if the operation was successful</param>
        /// <param name="error">Any error that occurred updating the object</param>
        public UpdateObjectResponse(UpdateObjectRequest<T, TId> request, bool success, string error = null) :
            base(request, error)
        {
            Success = success;
        }
    }
}