using System.Threading.Tasks;

namespace LogicMine.Routing
{
    /// <summary>
    /// Takes a raw request and routes it to a place that can generate a response
    /// </summary>
    /// <typeparam name="TRawRequest">The raw request type, e.g. JObject, string, etc</typeparam>
    public interface IRequestRouter<in TRawRequest>
    {
        /// <summary>
        /// Route the raw request through the system so that a response may be gained
        /// </summary>
        /// <param name="rawRequest">The raw request</param>
        /// <returns>A response to the request</returns>
        Task<IResponse> RouteAsync(TRawRequest rawRequest);
    }
}