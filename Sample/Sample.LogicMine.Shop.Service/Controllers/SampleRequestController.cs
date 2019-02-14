using LogicMine;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Shop.Service.Controllers
{
    // The default JsonRequestController is good enough for our sample (and should be good enough for most services).
    [Route("api")]
    public class SampleRequestController : JsonRequestController
    {
        public SampleRequestController(IRequestRouter<JObject> requestRouter, IErrorExporter errorExporter) :
            base(requestRouter, errorExporter)
        {
        }
    }
}