using System;

namespace LogicMine.Web.Request
{
    public abstract class RequestParser<TRawRequest> : IRequestParser<TRawRequest>
    {
        public abstract string HandledRequestType { get; }

        protected abstract string GetRequestType(TRawRequest rawRequest);
        public abstract IRequest Parse(TRawRequest rawRequest);

        public bool CanHandleRequest(TRawRequest rawRequest)
        {
            var requestType = GetRequestType(rawRequest);
            return HandledRequestType == requestType;
        }

        public TRequest Parse<TRequest>(TRawRequest rawRequest)
        {
            var request = Parse(rawRequest);
            if (!(request is TRequest castRequest))
            {
                throw new InvalidOperationException(
                    $"Expected the request string to parse to a '{typeof(TRequest)}' but it was a '{request?.GetType()}'");
            }

            return castRequest;
        }
    }
}