namespace LogicMine.DataObject.GetObject
{
    /// <summary>
    /// A response to a GetObjectRequest
    /// </summary>
    /// <typeparam name="T">The data type requested</typeparam>
    public class GetObjectResponse<T> : Response
    {
        /// <summary>
        /// The data type requested
        /// </summary>
        public string ObjectType { get; }

        /// <summary>
        /// The resulting object
        /// </summary>
        public T Object { get; }

        /// <summary>
        /// Construct a GetObjectResponse 
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public GetObjectResponse(IRequest request) : base(request)
        {
        }

        /// <summary>
        /// Construct a GetObjectResponse 
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="obj">The resulting object</param>
        public GetObjectResponse(IRequest request, T obj) : base(request)
        {
            ObjectType = typeof(T).Name;
            Object = obj;
        }
    }
}