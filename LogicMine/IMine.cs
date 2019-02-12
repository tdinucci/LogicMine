using System.Threading.Tasks;

namespace LogicMine
{
    /// <summary>
    /// Represents a collection of shafts
    /// </summary>
    public interface IMine
    {
        /// <summary>
        /// Add a shaft to the mine
        /// </summary>
        /// <param name="shaft">The shaft to add</param>
        /// <returns>A reference to the mine</returns>
        IMine AddShaft(IShaft shaft);

        /// <summary>
        /// Sends a request into the mine, the mine will choose the correct shaft to dispatch it to.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The response</returns>
        Task<IResponse> SendAsync(IRequest request);

        /// <summary>
        /// Sends a request into the mine, the mine will choose the correct shaft to dispatch it to.
        /// </summary>
        /// <param name="request">The request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns>The response</returns>
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest
            where TResponse : IResponse;
    }
}