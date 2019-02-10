using System;
using System.Threading.Tasks;

namespace LogicMine
{
    public interface IStation<TRequest, TResponse> : IStation
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        Task DescendToAsync(IBasket basket, IBasketPayload<TRequest, TResponse> payload);
        Task AscendFromAsync(IBasket basket, IBasketPayload<TRequest, TResponse> payload);
    }

    public interface IStation
    {
        Type RequestType { get; }
        Type ResponseType { get; }

        Task DescendToAsync(IBasket basket);
        Task AscendFromAsync(IBasket basket);
    }
}