using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Resilience.Controllers
{
    [Route("api")]
    public class MyRequestController : JsonRequestController
    {
        public MyRequestController(IRequestRouter<JObject> requestRouter) : base(requestRouter)
        {
        }
    }
}