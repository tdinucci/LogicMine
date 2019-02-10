using System;
using System.Threading.Tasks;

namespace LogicMine
{
    public abstract class Station<TRequest, TResponse> : IStation<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        public Type RequestType { get; } = typeof(TRequest);
        public Type ResponseType { get; } = typeof(TResponse);

        public abstract Task DescendToAsync(IBasket basket, IBasketPayload<TRequest, TResponse> payload);
        public abstract Task AscendFromAsync(IBasket basket, IBasketPayload<TRequest, TResponse> payload);

        Task IStation.DescendToAsync(IBasket basket)
        {
            return DescendToAsync(basket, UnwrapBasketPayload(basket));
        }

        Task IStation.AscendFromAsync(IBasket basket)
        {
            return AscendFromAsync(basket, UnwrapBasketPayload(basket));
        }

        protected IBasketPayload<TRequest, TResponse> UnwrapBasketPayload(IBasket basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            return basket.Payload?.Unwrap<TRequest, TResponse>();
        }
    }
}