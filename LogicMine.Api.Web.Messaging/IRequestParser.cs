using LogicMine.Api.Web.Messaging.Request;

namespace LogicMine.Api.Web.Messaging
{
    public interface IRequestParser
    {
        IRequest Parse(string serialisedRequest);
        RequestParser Register<TRequest>() where TRequest : IRequest;
    }
}