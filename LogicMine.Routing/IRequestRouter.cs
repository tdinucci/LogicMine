using System.Threading.Tasks;

namespace LogicMine.Routing
{
    public interface IRequestRouter<in TRawRequest>
    {
        Task<IResponse> RouteAsync(TRawRequest rawRequest);
    }
}