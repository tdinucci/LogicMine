using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LogicMine.Web.Request
{
    public abstract class RequestParser<TRawRequest> : IRequestParser<TRawRequest>
    {
        private readonly HashSet<string> _handledRequestTypes = new HashSet<string>();

        public ImmutableHashSet<string> HandledRequestTypes => _handledRequestTypes.ToImmutableHashSet();
        
        protected abstract string GetRequestType(TRawRequest rawRequest);
        public abstract IRequest Parse(TRawRequest rawRequest);

        protected void AddHandledRequestType(params string[] requestTypeNames)
        {
            if (requestTypeNames != null)
            {
                foreach (var requestTypeName in requestTypeNames)
                    _handledRequestTypes.Add(requestTypeName.ToLower());
            }
        }

        public bool CanHandleRequest(TRawRequest rawRequest)
        {
            var requestType = GetRequestType(rawRequest);
            return requestType != null && _handledRequestTypes.Contains(requestType.ToLower());
        }

        public void EnsureCanHandleRequest(TRawRequest rawRequest)
        {
            if (!CanHandleRequest(rawRequest))
            {
                throw new InvalidOperationException(
                    $"This parser handles '{GetHandledRequestTypesDescription()}' not '{GetRequestType(rawRequest)}'");
            }
        }

        protected string GetHandledRequestTypesDescription()
        {
            return _handledRequestTypes.Aggregate((c, n) => $"{c},{n}");
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