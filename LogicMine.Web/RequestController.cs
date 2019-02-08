using System;
using System.Threading.Tasks;
using LogicMine.Web.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LogicMine.Web
{
    public class RequestController : Controller
    {
        private readonly IMine _mine;
        private readonly IRequestParserRegistry<JObject> _parserRegistry;

        public RequestController(IMine mine, IRequestParserRegistry<JObject> parserRegistry)
        {
            _mine = mine ?? throw new ArgumentNullException(nameof(mine));
            _parserRegistry = parserRegistry ?? throw new ArgumentNullException(nameof(parserRegistry));
        }

        [HttpPost]
        public async Task<IActionResult> PostRequest([FromBody] JObject request)
        {
            var parsedRequest = _parserRegistry.Get(request).Parse(request);

            var response = await _mine.SendAsync(parsedRequest);

            if (!string.IsNullOrWhiteSpace(response.Error))
                return BadRequest(response);

            return Ok(response);
        }
    }
}