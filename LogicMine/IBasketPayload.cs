using System;

namespace LogicMine
{
    public interface IBasketPayload<out TRequest, TResponse> : IBasketPayload
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        new TRequest Request { get; }
        new TResponse Response { get; set; }
    }

    public interface IBasketPayload
    {
        Type RequestType { get; }
        Type ResponseType { get; }

        IRequest Request { get; }
        IResponse Response { get; set; }

        IBasketPayload<TRequest, TResponse> Unwrap<TRequest, TResponse>()
            where TRequest : class, IRequest
            where TResponse : IResponse;
    }
}