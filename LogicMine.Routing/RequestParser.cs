using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LogicMine.Routing
{
    /// <inheritdoc />
    public abstract class RequestParser<TRawRequest> : IRequestParser<TRawRequest>
    {
        private readonly HashSet<string> _handledRequestTypes = new HashSet<string>();

        /// <inheritdoc />
        public ImmutableHashSet<string> HandledRequestTypes => _handledRequestTypes.ToImmutableHashSet();

        /// <summary>
        /// Returns the type of request which rawRequest contains
        /// </summary>
        /// <param name="rawRequest">The request to determine the type of</param>
        /// <returns></returns>
        protected abstract string GetRequestType(TRawRequest rawRequest);

        /// <inheritdoc />
        public abstract IRequest Parse(TRawRequest rawRequest);

        /// <summary>
        /// Add the names of the request types which this parser can handle
        /// </summary>
        /// <param name="requestTypeNames"></param>
        protected void AddHandledRequestType(params string[] requestTypeNames)
        {
            if (requestTypeNames != null)
            {
                foreach (var requestTypeName in requestTypeNames)
                    _handledRequestTypes.Add(requestTypeName.ToLower());
            }
        }

        /// <inheritdoc />
        public bool CanHandleRequest(TRawRequest rawRequest)
        {
            var requestType = GetRequestType(rawRequest);
            return requestType != null && _handledRequestTypes.Contains(requestType.ToLower());
        }

        /// <inheritdoc />
        public void EnsureCanHandleRequest(TRawRequest rawRequest)
        {
            if (!CanHandleRequest(rawRequest))
            {
                throw new InvalidOperationException(
                    $"This parser handles '{GetHandledRequestTypesDescription()}' not '{GetRequestType(rawRequest)}'");
            }
        }

        /// <summary>
        /// Get a description of the request types that are handled by this parser
        /// </summary>
        /// <returns></returns>
        protected string GetHandledRequestTypesDescription()
        {
            return _handledRequestTypes.Aggregate((c, n) => $"{c},{n}");
        }

        /// <inheritdoc />
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