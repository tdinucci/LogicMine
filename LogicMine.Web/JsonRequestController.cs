using System;
using System.Collections.Generic;
using System.Linq;
using LogicMine.Routing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LogicMine.Web
{
    /// <summary>
    /// A Web API controller which accepts raw JObject requests.  These raw requests will then
    /// be dispatched to the injected IRequestRouter and this will in turn ensure they are parsed and
    /// passed to the appropriate mine and shaft so that a response may be obtained.
    /// </summary>
    public class JsonRequestController : RequestController<JObject>
    {
        private const string RequestIdField = "requestId";
        private const string DateField = "date";
        private const string ErrorField = "error";

        /// <summary>
        /// Construct a JsonRequestController
        /// </summary>
        /// <param name="requestRouter">The router which takes raw requests and dispatches them to a mine</param>
        /// <param name="errorExporter">The error exporter to use when errors occur</param>
        public JsonRequestController(IRequestRouter<JObject> requestRouter, IErrorExporter errorExporter = null) :
            base(requestRouter, errorExporter)
        {
        }

        /// <inheritdoc />
        protected override IActionResult GetActionResult(JObject request, IResponse response)
        {
            var jsonResponse = JObject.FromObject(response, JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            if (!string.IsNullOrWhiteSpace(response.Error))
                return GetErrorResponse(jsonResponse);

            RemoveUnselectedProperties(request, jsonResponse);
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

        // TODO: This is horribly hacky, do something nicer so this class doesn't have to know anything about specific requests/responses
        // Purpose of this is to "prettify" the result, the unselected properties will be null in the result but it's nicer
        // to not even return this to the client
        private void RemoveUnselectedProperties(JObject request, JObject response)
        {
            if (request.ContainsKey("requestType"))
            {
                var requestType = request["requestType"].Value<string>();
                var isGetObjectRequest = string.Equals(requestType, "getObject", StringComparison.OrdinalIgnoreCase);
                var isGetCollectionRequest = !isGetObjectRequest && string.Equals(requestType, "getCollection",
                                                 StringComparison.OrdinalIgnoreCase);

                if (isGetObjectRequest || isGetCollectionRequest)
                {
                    if (request.ContainsKey("select") && request["select"].HasValues)
                    {
                        var selected = request["select"].Select(t => t.Value<string>()).ToArray();
                        if (selected.Any())
                        {
                            if (isGetObjectRequest)
                            {
                                if (response["object"] is JObject jObject)
                                    RemoveUnselectedPropertiesFromJObject(jObject, selected);
                            }
                            else if (response["objects"] is JArray objs)
                            {
                                foreach (var token in objs)
                                {
                                    if (token is JObject jObject)
                                        RemoveUnselectedPropertiesFromJObject(jObject, selected);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveUnselectedPropertiesFromJObject(JObject jObject, string[] selected)
        {
            var toRemove = new List<string>();
            foreach (var child in jObject.Children())
            {
                if (child is JProperty prop &&
                    !selected.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                {
                    toRemove.Add(prop.Name);
                }
            }

            foreach (var remove in toRemove)
                jObject.Remove(remove);
        }
    }
}