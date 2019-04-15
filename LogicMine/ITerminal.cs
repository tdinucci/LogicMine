using System.Threading.Tasks;

namespace LogicMine
{
    /// <summary>
    /// An internal interface which allows for certain properties to be manipulated from types within this library
    /// </summary>
    internal interface IInternalTerminal : ITerminal
    {
        new IShaft Within { get; set; }
    }
    
    /// <inheritdoc />
    public interface ITerminal<in TRequest, TResponse> : ITerminal
        where TRequest : class, IRequest
        where TResponse : IResponse<TRequest>
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
        /// The shaft this station is within
        /// </summary>
        IShaft Within { get; }
        
        /// <summary>
        /// Add a response to the basket
        /// </summary>
        /// <param name="basket">The basket to add the response to</param>
        /// <returns></returns>
        Task AddResponseAsync(IBasket basket);
    }
}