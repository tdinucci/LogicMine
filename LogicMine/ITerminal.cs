using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc />
    public interface ITerminal<in TRequest, TResponse> : ITerminal
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <summary>
        /// Add a response to the basket
        /// </summary>
        /// <param name="basket">The basket to add the response to</param>
        /// <returns></returns>
        Task AddResponseAsync(IBasket<TRequest, TResponse> basket);
    }

    /// <summary>
    /// The lowest waypoint within a shaft which adds a response to a basket
    /// </summary>
    public interface ITerminal
    {
        /// <summary>
        /// Add a response to the basket
        /// </summary>
        /// <param name="basket">The basket to add the response to</param>
        /// <returns></returns>
        Task AddResponseAsync(IBasket basket);
    }
}