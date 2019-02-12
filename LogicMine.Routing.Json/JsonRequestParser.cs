using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json
{
    /// <summary>
    /// A <see cref="RequestParser{TRawRequest}"/> that specialises in parsing JObjects
    /// </summary>
    public abstract class JsonRequestParser : RequestParser<JObject>
    {
        public const string RequestTypeField = "requestType";

        protected override string GetRequestType(JObject rawRequest)
        {
            if (rawRequest.ContainsKey(RequestTypeField))
                return rawRequest[RequestTypeField].Value<string>();

            return null;
        }
    }
}