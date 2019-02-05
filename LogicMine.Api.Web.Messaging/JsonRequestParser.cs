using System;
using LogicMine.Api.Web.Messaging.Request;
using Newtonsoft.Json.Linq;

namespace LogicMine.Api.Web.Messaging
{
    public class JsonRequestParser : RequestParser
    {
        private const string RequestTypeField = "requestType";

        public override IRequest Parse(string serialisedRequest)
        {
            if (string.IsNullOrWhiteSpace(serialisedRequest))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serialisedRequest));

            var jobj = JObject.Parse(serialisedRequest);
            if (!jobj.ContainsKey(RequestTypeField))
                throw new InvalidOperationException($"Request does not contain a '{RequestTypeField}'");

            var request = GetUninitialisedRequest(jobj[RequestTypeField].Value<string>());
            request.Initialise(jobj);

            return request;
        }
    }
}