using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc cref="IStation{TRequest,TResponse}" />
    public abstract class Station<TRequest, TResponse> : IStation<TRequest, TResponse>, IInternalStation
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <inheritdoc cref="IStation{TRequest,TResponse}" />
        public IShaft Within { get; set; }

        /// <inheritdoc />
        public Type RequestType { get; } = typeof(TRequest);

        /// <inheritdoc />
        public Type ResponseType { get; } = typeof(TResponse);

        /// <inheritdoc />
        public abstract Task DescendToAsync(IBasket<TRequest, TResponse> basket);

        /// <inheritdoc />
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