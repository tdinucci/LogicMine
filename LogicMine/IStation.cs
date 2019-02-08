using System;
using System.Threading.Tasks;

namespace LogicMine
{
    public interface IStation<TRequest, TResponse> : IStation
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
//        Task DescendToAsync(IBasket<TRequest, TResponse> basket);
//        Task AscendFromAsync(IBasket<TRequest, TResponse> basket);

        IBasketPayload<TRequest, TResponse> UnwrapBasketPayload(IBasket basket);
    }

    public interface IStation
    {
        Type RequestType { get; }
        Type ResponseType { get; }

        Task DescendToAsync(IBasket basket);
        Task AscendFromAsync(IBasket basket);
    }
}