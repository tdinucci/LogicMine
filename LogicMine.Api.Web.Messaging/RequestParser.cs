using System;
using System.Collections.Generic;
using LogicMine.Api.Web.Messaging.Request;

namespace LogicMine.Api.Web.Messaging
{
    public abstract class RequestParser : IRequestParser
    {
        private static readonly object RegisterLock = new object();

        private readonly Dictionary<string, Type> _requestTypes = new Dictionary<string, Type>();

        public abstract IRequest Parse(string serialisedRequest);

        public RequestParser Register<TRequest>() where TRequest : IRequest
        {
            var requestType = typeof(TRequest);
            if (_requestTypes.ContainsKey(requestType.Name))
                throw new InvalidOperationException($"Request type called '{requestType.Name}' already registered");

            lock (RegisterLock)
            {
                if (!_requestTypes.ContainsKey(requestType.Name))
                    _requestTypes.Add(requestType.Name, requestType);
            }

            return this;
        }

        protected IRequest GetUninitialisedRequest(string requestTypeName)
        {
            if (!_requestTypes.ContainsKey(requestTypeName))
                throw new InvalidOperationException($"There is no request type called '{requestTypeName}'");

            return (IRequest) Activator.CreateInstance(_requestTypes[requestTypeName]);
        }
    }
}