using System.Collections.Immutable;

namespace LogicMine.Routing
{
    /// <summary>
    /// A parser for requests
    /// </summary>
    /// <typeparam name="TRawRequest">The request type, e.g. JObject, string, etc.</typeparam>
    public interface IRequestParser<in TRawRequest>
    {
        /// <summary>
        /// The set of request types which the parser can cope with
        /// </summary>
        ImmutableHashSet<string> HandledRequestTypes { get; }

        /// <summary>
        /// Returns true if the parser can handle the given request
        /// </summary>
        /// <param name="rawRequest">The request to check</param>
        /// <returns>True if the parser can handle the request</returns>
        bool CanHandleRequest(TRawRequest rawRequest);

        /// <summary>
        /// Performs the same check as <see cref="CanHandleRequest"/> however throws and
        /// exception if the request cannot be handled
        /// </summary>
        /// <param name="rawRequest">The request to check</param>
        void EnsureCanHandleRequest(TRawRequest rawRequest);

        /// <summary>
        /// Parse the given request
        /// </summary>
        /// <param name="rawRequest">The request to parse</param>
        /// <returns>A parsed IRequest</returns>
        IRequest Parse(TRawRequest rawRequest);

        /// <summary>
        /// Parse the given request into a TRequest
        /// </summary>
        /// <param name="rawRequest">The request to parse</param>
        /// <typeparam name="TRequest">The type of IRequest to parse into</typeparam>
        /// <returns>A parsed TRequest</returns>
        TRequest Parse<TRequest>(TRawRequest rawRequest);
    }
}