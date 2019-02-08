using System.Threading.Tasks;

namespace LogicMine
{
    public interface IMine
    {
        IMine AddShaft(IShaft shaft);

        Task<IResponse> SendAsync(IRequest request);

        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest
            where TResponse : IResponse, new();
    }
}