using System;
using Newtonsoft.Json.Linq;

namespace LogicMine.Web.Request
{
    public class GetObjectRequestJsonParser : JsonRequestParser
    {
        public override string HandledRequestType { get; } = "getObject";

        public override IRequest Parse(JObject rawRequest)
        {
            if (!CanHandleRequest(rawRequest))
            {
                throw new InvalidOperationException(
                    $"This parser handles '{HandledRequestType}' not '{GetRequestType(rawRequest)}'");
            }

            return null;//new GetObjectRequest<T,TId>();
        }
    }
}