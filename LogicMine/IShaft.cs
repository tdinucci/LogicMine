using System;
using System.Threading.Tasks;

namespace LogicMine
{
    public interface IShaft<TRequest, TResponse> : IShaft
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        Task<TResponse> SendAsync(TRequest request);
    }

    public interface IShaft
    {
        Type RequestType { get; }
        Type ResponseType { get; }

        void AddToTop(params IStation[] stations);
        void AddToBottom(params IStation[] stations);

        Task<IResponse> SendAsync(IRequest request);
    }
}