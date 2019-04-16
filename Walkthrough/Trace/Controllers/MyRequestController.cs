using LogicMine;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Trace.Controllers
{
    [Route("api")]
    public class MyRequestController : JsonRequestController
    {
        public MyRequestController(IRequestRouter<JObject> requestRouter, IErrorExporter errorExporter) :
            base(requestRouter, errorExporter)
        {
        }
    }
}