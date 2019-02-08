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

        public abstract Task DescendToAsync(IBasket basket);
        public abstract Task AscendFromAsync(IBasket basket);

        public IBasketPayload<TRequest, TResponse> UnwrapBasketPayload(IBasket basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            return basket.Payload?.Unwrap<TRequest, TResponse>();
        }
    }

//    public abstract class Station : IStation
//    {
//        public Type RequestType { get; }
//        public Type ResponseType { get; }
//
//        protected Station(Type requestType, Type responseType)
//        {
//            RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
//            ResponseType = responseType ?? throw new ArgumentNullException(nameof(responseType));
//        }
//
//        protected abstract Task PerformDescendOperationAsync(IBasket basket);
//        protected abstract Task PerformAscendOperationAsync(IBasket basket);
//
//        public Task DescendToAsync(IBasket basket)
//        {
//            EnsureRequestAndResponseTypesValid(basket);
//
//            return PerformDescendOperationAsync(basket);
//        }
//
//        public Task AscendFromAsync(IBasket basket)
//        {
//            EnsureRequestAndResponseTypesValid(basket);
//
//            return PerformAscendOperationAsync(basket);
//        }
//
//        private void EnsureRequestAndResponseTypesValid(IBasket basket)
//        {
//            if (!RequestType.IsAssignableFrom(basket.Payload.RequestType))
//            {
//                throw new InvalidOperationException(
//                    $"The baskets request type of '{basket.Payload.RequestType}' is not compatible with the stations request type of '{RequestType}'");
//            }
//
//            if (!ResponseType.IsAssignableFrom(basket.Payload.ResponseType))
//            {
//                throw new InvalidOperationException(
//                    $"The baskets response type of '{basket.Payload.ResponseType}' is not compatible with the stations response type of '{ResponseType}'");
//            }
//        }
//    }
}