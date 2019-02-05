using System.Threading.Tasks;
using LogicMine.Api.Web.Messaging.Request;
using LogicMine.Api.Web.Messaging.Response;

namespace LogicMine.Api.Web.Messaging.Handler
{
    public interface IRequestRouter
    {
        Task<IResponse> RouteAsync(IRequest request);
    }
}