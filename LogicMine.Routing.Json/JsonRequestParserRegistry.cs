using System;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json
{
    public class JsonRequestParserRegistry : RequestParserRegistry<JObject>
    {
        protected override string GetRequestType(JObject rawRequest)
        {
            if (rawRequest.ContainsKey(JsonRequestParser.RequestTypeField))
                return rawRequest[JsonRequestParser.RequestTypeField].Value<string>().ToLower();

            throw new InvalidOperationException("Could not determine request type");
        }
    }
}