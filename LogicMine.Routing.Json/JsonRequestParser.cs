using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json
{
    /// <summary>
    /// A <see cref="RequestParser{TRawRequest}"/> that specialises in parsing JObjects
    /// </summary>
    public abstract class JsonRequestParser : RequestParser<JObject>
    {
        /// <summary>
        /// The field within a JObject request which specifies the request type
        /// </summary>
        public const string RequestTypeField = "requestType";

        /// <inheritdoc />
        protected override string GetRequestType(JObject rawRequest)
        {
            if (rawRequest.ContainsKey(RequestTypeField))
                return rawRequest[RequestTypeField].Value<string>();

            return null;
        }
    }
}