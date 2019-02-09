using LogicMine;
using LogicMine.Web;
using LogicMine.Web.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Web
{
    [Route("api")]
    public class SampleRequestController : RequestController
    {
        public SampleRequestController(IHttpContextAccessor httpContextAccessor, IMine mine,
            IRequestParserRegistry<JObject> parserRegistry) :
            base(httpContextAccessor, mine, parserRegistry)
        {
        }
    }
}