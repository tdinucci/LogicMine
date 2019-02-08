using System.Threading.Tasks;

namespace LogicMine
{
    public interface ITerminal<in TRequest, TResponse> : ITerminal
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        Task AddResponseAsync(IBasket<TRequest, TResponse> basket);
    }

    public interface ITerminal
    {
        Task AddResponseAsync(IBasket basket);
    }
}