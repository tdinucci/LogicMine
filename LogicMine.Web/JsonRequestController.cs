using LogicMine.Routing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LogicMine.Web
{
    public class JsonRequestController : RequestController<JObject>
    {
        private const string RequestIdField = "requestId";
        private const string DateField = "date";
        private const string ErrorField = "error";

        public JsonRequestController(IRequestRouter<JObject> requestRouter, IErrorExporter errorExporter) :
            base(requestRouter, errorExporter)
        {
        }

        protected override IActionResult GetActionResult(IResponse response)
        {
            var jsonResponse = JObject.FromObject(response, JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            if (!string.IsNullOrWhiteSpace(response.Error))
                return GetErrorResponse(jsonResponse);

            return GetSuccessResponse(jsonResponse);
        }

        private IActionResult GetSuccessResponse(JObject response)
        {
            if (response.ContainsKey(ErrorField))
                response.Remove(ErrorField);

            return Ok(response);
        }

        private IActionResult GetErrorResponse(JObject response)
        {
            var result = new JObject();

            if (response.ContainsKey(RequestIdField))
                result.Add(RequestIdField, response[RequestIdField]);
            if (response.ContainsKey(DateField))
                result.Add(DateField, response[DateField]);
            if (response.ContainsKey(ErrorField))
                result.Add(ErrorField, response[ErrorField]);

            return new InternalServerErrorObjectResult(result);
        }
    }
}