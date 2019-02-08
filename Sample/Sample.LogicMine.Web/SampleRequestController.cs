using LogicMine;
using LogicMine.Web;
using LogicMine.Web.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Web
{
    [Route("api")]
    public class SampleRequestController : RequestController
    {
        public SampleRequestController(IMine mine, IRequestParserRegistry<JObject> parserRegistry) :
            base(mine, parserRegistry)
        {
        }
    }
}