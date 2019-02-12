using System;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json
{
    /// <summary>
    /// A <see cref="RequestParserRegistry{TRawRequest}"/> that specialises in parsers that
    /// handle JObjects
    /// </summary>
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