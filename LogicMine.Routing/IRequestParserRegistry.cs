namespace LogicMine.Routing
{
    /// <summary>
    /// A registry for parsers which handle requests of type TRawRequest
    /// </summary>
    /// <typeparam name="TRawRequest">The raw request type, e.g. JObject, string, etc</typeparam>
    public interface IRequestParserRegistry<TRawRequest>
    {
        /// <summary>
        /// Register a parser with the registry
        /// </summary>
        /// <param name="parser">The parser to register</param>
        /// <returns>A reference to "this" registry</returns>
        IRequestParserRegistry<TRawRequest> Register(IRequestParser<TRawRequest> parser);

        /// <summary>
        /// Retrieve a parser from the registry that can handle the given request
        /// </summary>
        /// <param name="request">The request to find a parser for</param>
        /// <returns>A parser than can handle the given request</returns>
        IRequestParser<TRawRequest> Get(TRawRequest request);
    }
}