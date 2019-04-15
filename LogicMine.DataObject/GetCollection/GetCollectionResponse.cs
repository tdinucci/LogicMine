namespace LogicMine.DataObject.GetCollection
{
    /// <summary>
    /// A response to a GetCollectionRequest
    /// </summary>
    /// <typeparam name="T">The requested type</typeparam>
    public class GetCollectionResponse<T> : Response<GetCollectionRequest<T>>
    {
        /// <summary>
        /// The requested type
        /// </summary>
        public string ObjectType { get; }

        /// <summary>
        /// The objects which were retrieved 
        /// </summary>
        public T[] Objects { get; }

        /// <summary>
        /// Construct a GetCollectionResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public GetCollectionResponse(GetCollectionRequest<T> request) : base(request)
        {
        }

        /// <summary>
        /// Construct a GetCollectionResponse
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="objects">The objects which were retrieved</param>
        public GetCollectionResponse(GetCollectionRequest<T> request, T[] objects) : base(request)
        {
            ObjectType = typeof(T).Name;
            Objects = objects;
        }
    }
}