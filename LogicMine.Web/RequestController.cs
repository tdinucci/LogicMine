using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LogicMine.Web.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LogicMine.Web
{
    public class RequestController : Controller
    {
        private const string AuthorisationHeaderName = "Authorization";
        private const string BearerSchemeName = "Bearer";

        private readonly IMine _mine;
        private readonly IRequestParserRegistry<JObject> _parserRegistry;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITraceExporter _traceExporter;

        public RequestController(IHttpContextAccessor httpContextAccessor, IMine mine,
            IRequestParserRegistry<JObject> parserRegistry, ITraceExporter traceExporter)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mine = mine ?? throw new ArgumentNullException(nameof(mine));
            _parserRegistry = parserRegistry ?? throw new ArgumentNullException(nameof(parserRegistry));
            _traceExporter = traceExporter;
        }

        [HttpPost]
        public async Task<IActionResult> PostRequest([FromBody] JObject request)
        {
            IRequest parsedRequest = null;
            try
            {
                if (request == null)
                    throw new ArgumentException("Request body was not provided or could not be parsed");

                parsedRequest = _parserRegistry.Get(request).Parse(request);
                parsedRequest.Options.Add("AccessToken", GetAccessToken());

                var response = await _mine.SendAsync(parsedRequest).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(response.Error))
                    return new InternalServerErrorObjectResult(new Response(parsedRequest, response.Error));

                var jsonResponse = JObject.FromObject(response, JsonSerializer.Create(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
                jsonResponse.Remove("error");

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _traceExporter?.ExportError(ex);

                if (ex.InnerException != null)
                    ex = ex.InnerException;

                return new InternalServerErrorObjectResult(new Response(parsedRequest, ex.Message));
            }
        }

        private string GetAccessToken()
        {
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers[AuthorisationHeaderName].FirstOrDefault();
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
                if (authHeaderVal.Scheme.Equals(BearerSchemeName, StringComparison.OrdinalIgnoreCase))
                    return Uri.UnescapeDataString(authHeaderVal.Parameter);
            }

            return null;
        }
    }
}