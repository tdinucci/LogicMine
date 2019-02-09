using System;
using System.Threading.Tasks;

namespace LogicMine
{
    public interface IShaft<TRequest, TResponse> : IShaft
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        new IShaft<TRequest, TResponse> AddToTop(params IStation[] stations);
        new IShaft<TRequest, TResponse> AddToBottom(params IStation[] stations);
        
        Task<TResponse> SendAsync(TRequest request);
    }

    public interface IShaft
    {
        Type RequestType { get; }
        Type ResponseType { get; }

        IShaft AddToTop(params IStation[] stations);
        IShaft AddToBottom(params IStation[] stations);

        Task<IResponse> SendAsync(IRequest request);
    }
}