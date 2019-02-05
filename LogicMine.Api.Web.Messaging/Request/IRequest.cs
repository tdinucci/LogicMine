using Newtonsoft.Json.Linq;

namespace LogicMine.Api.Web.Messaging.Request
{
    public interface IRequest
    {
        void Initialise(JObject jobj);
    }
}