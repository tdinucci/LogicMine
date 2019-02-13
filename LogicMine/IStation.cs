using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <summary>
    /// An internal interface which allows for certain properties to be manipulated from types within this library
    /// </summary>
    internal interface IInternalStation : IStation
    {
        new IShaft Within { get; set; }
    }

    /// <inheritdoc />
    /// <typeparam name="TRequest">The type of request handled by the station</typeparam>
    /// <typeparam name="TResponse">The type of response handled by the station</typeparam>
    public interface IStation<in TRequest, TResponse> : IStation
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <summary>
        /// Act on a basket on it's way down a shaft.
        /// </summary>
        /// <param name="basket">The basket to act on</param>
        /// <returns></returns>
        Task DescendToAsync(IBasket<TRequest, TResponse> basket);

        /// <summary>
        /// Act on a basket on it's way back up a shaft.
        /// </summary>
        /// <param name="basket">The basket to act on</param>
        /// <returns></returns>
        Task AscendFromAsync(IBasket<TRequest, TResponse> basket);
    }

    /// <summary>
    /// A waypoint on within a shaft which baskets pass through both on the downward and upward journeys.
    ///
    /// When a basket descends through a station the response is not yet known and station will typically 
    /// act on the request.
    ///
    /// When a basket ascends through a station the response will be known and the station will typically 
    /// act on the response.
    /// </summary>
    public interface IStation
    {
        /// <summary>
        /// The shaft this station is within
        /// </summary>
        IShaft Within { get; }

        /// <summary>
        /// The type of request handled by the station
        /// </summary>
        Type RequestType { get; }

        /// <summary>
        /// The type of response handled by the station
        /// </summary>
        Type ResponseType { get; }

        /// <summary>
        /// Act on a basket on it's way down a shaft
        /// </summary>
        /// <param name="basket">The basket to act on</param>
        /// <returns></returns>
        Task DescendToAsync(ref IBasket basket);

        /// <summary>
        /// Act on a basket on it's way back up a shaft
        /// </summary>
        /// <param name="basket">The basket to act on</param>
        /// <returns></returns>
        Task AscendFromAsync(ref IBasket basket);
    }
}