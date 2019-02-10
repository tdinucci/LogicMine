using System;
using System.Collections.Generic;

namespace LogicMine.Routing
{
    public abstract class RequestParserRegistry<TRawRequest> : IRequestParserRegistry<TRawRequest>
    {
        private IDictionary<string, IRequestParser<TRawRequest>> Parsers { get; } =
            new Dictionary<string, IRequestParser<TRawRequest>>();

        protected abstract string GetRequestType(TRawRequest request);

        public IRequestParserRegistry<TRawRequest> Register(IRequestParser<TRawRequest> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            foreach (var handledRequestType in parser.HandledRequestTypes)
            {
                if (Parsers.ContainsKey(handledRequestType))
                {
                    throw new InvalidOperationException(
                        $"There is already a parser registered for '{handledRequestType}' requests");
                }
                
                Parsers.Add(handledRequestType, parser);
            }
            
            return this;
        }

        public IRequestParser<TRawRequest> Get(TRawRequest request)
        {
            var requestType = GetRequestType(request);
            if (!Parsers.ContainsKey(requestType))
                throw new InvalidOperationException($"There is no parser registered for '{requestType}' requests");

            return Parsers[requestType];
        }
    }
}