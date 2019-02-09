using System;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionRequest<T> : Request
    {
        public Type ObjectType { get; } = typeof(T);
        
        public IFilter<T> Filter { get; }

        public int? Max { get; }

        public int? Page { get; }

        /// <summary>
        /// Construct a new GetCollectionRequest
        /// </summary>
        public GetCollectionRequest() : this(null, null, null)
        {
        }

        /// <summary>
        /// Construct a new GetCollectionRequest
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        public GetCollectionRequest(IFilter<T> filter) : this(filter, null, null)
        {
        }

        /// <summary>
        /// Construct a new GetCollectionRequest
        /// </summary>
        /// <param name="max">The maximum number of results to return</param>
        /// <param name="page">The page within the results</param>
        public GetCollectionRequest(int max, int page) : this(null, max, page)
        {
        }

        /// <summary>
        /// Construct a new GetCollectionRequest
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        /// <param name="max">The maximum number of results to return</param>
        /// <param name="page">The page within the results</param>
        public GetCollectionRequest(IFilter<T> filter, int? max, int? page)
        {
            if (max.GetValueOrDefault(1) <= 0)
                throw new ArgumentException($"The value for '{nameof(max)}' must be greater than 0");
            if (page.GetValueOrDefault(1) < 0)
                throw new ArgumentException($"The value for '{nameof(page)}' must be 0 or greater");

            Filter = filter;
            Max = max;
            Page = page;
        }
    }
}