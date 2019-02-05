using System;
using System.Threading.Tasks;
using LogicMine.Api.Web.Messaging.Request;
using LogicMine.Api.Web.Messaging.Response;

namespace LogicMine.Api.Web.Messaging.Handler
{
    public class RequestRouter : IRequestRouter
    {
        //private Dictionary<Type, I>

        public Task<IResponse> RouteAsync(IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}