using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc />
    public abstract class Station<TRequest, TResponse> : IStation<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <inheritdoc />
        public Type RequestType { get; } = typeof(TRequest);

        /// <inheritdoc />
        public Type ResponseType { get; } = typeof(TResponse);

        /// <inheritdoc />
        public abstract Task DescendToAsync(IBasket<TRequest, TResponse> basket);

        /// <inheritdoc />
        public abstract Task AscendFromAsync(IBasket<TRequest, TResponse> basket);

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