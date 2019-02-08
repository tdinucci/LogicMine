using Newtonsoft.Json.Linq;

namespace LogicMine.Web.Request
{
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