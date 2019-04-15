using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc cref="IStation" />
    public abstract class FlexibleStation<TRequest, TResponse> : IStation, IInternalStation
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <inheritdoc cref="IStation" />
        public IShaft Within { get; set; }

        /// <inheritdoc />
        public Type RequestType { get; } = typeof(TRequest);

        /// <inheritdoc />
        public Type ResponseType { get; } = typeof(TResponse);

        /// <summary>
        /// Act on a basket on it's way down a shaft.
        /// </summary>
        /// <param name="basket">The basket to act on</param>
        /// <returns></returns>
        public abstract Task DescendToAsync(IBasket<TRequest, TResponse> basket);

        /// <summary>
        /// Act on a basket on it's way up a shaft.
        /// </summary>
        /// <param name="basket">The basket to act on</param>
        /// <returns></returns>
        public virtual Task AscendFromAsync(IBasket<TRequest, TResponse> basket)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        Task IStation.DescendToAsync(ref IBasket basket)
        {
            basket = Basket<TRequest, TResponse>.Copy(basket);
            return DescendToAsync((Basket<TRequest, TResponse>) basket);
        }

        /// <inheritdoc />
        Task IStation.AscendFromAsync(ref IBasket basket)
        {
            basket = Basket<TRequest, TResponse>.Copy(basket);
            return AscendFromAsync((Basket<TRequest, TResponse>) basket);
        }
    }
}