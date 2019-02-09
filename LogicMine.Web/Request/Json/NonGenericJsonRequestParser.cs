using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace LogicMine.Web.Request.Json
{
    public class NonGenericJsonRequestParser : JsonRequestParser
    {
        private readonly Dictionary<string, Type> _handledRequestTypes = new Dictionary<string, Type>();

        public NonGenericJsonRequestParser(params Type[] requestTypes)
        {
            InitialiseHandledRequestTypes(requestTypes);
        }

        private void InitialiseHandledRequestTypes(IEnumerable<Type> requestTypes)
        {
            if (requestTypes != null)
            {
                const string requestTypeNameSuffix = "request";
                foreach (var requestType in requestTypes)
                {
                    if (requestType.IsGenericType)
                    {
                        throw new InvalidOperationException(
                            $"The '{GetType()}' parser is not compatible with generic types");
                    }

                    var name = requestType.Name.ToLower();
                    if (name.EndsWith(requestTypeNameSuffix))
                        name = name.Remove(name.Length - requestTypeNameSuffix.Length);

                    if (_handledRequestTypes.ContainsKey(name))
                    {
                        throw new InvalidOperationException(
                            $"There is already a request type registered called '{requestType.Name}'");
                    }

                    AddHandledRequestType(name);
                    _handledRequestTypes.Add(name, requestType);
                }
            }
        }

        public override IRequest Parse(JObject rawRequest)
        {
            if (rawRequest == null) throw new ArgumentNullException(nameof(rawRequest));

            var requestTypeName = GetRequestType(rawRequest);
            if (string.IsNullOrEmpty(requestTypeName))
                throw new InvalidOperationException($"The request contained to '{RequestTypeField}'");

            if (!_handledRequestTypes.TryGetValue(requestTypeName.ToLower(), out var requestType))
                throw new InvalidOperationException($"There is no know request type called '{requestTypeName}'");

            return CreateRequest(requestType, rawRequest);
        }

        private IRequest CreateRequest(Type requestType, JObject rawRequest)
        {
            var ctor = requestType.GetConstructor(new Type[0]);
            if (ctor == null)
                throw new InvalidOperationException($"There is no default constructor on '{requestType}'");

            var request = (IRequest) ctor.Invoke(new object[0]);
            foreach (var rawProperty in rawRequest.Properties())
            {
                if (string.Equals(rawProperty.Name, RequestTypeField))
                    continue;

                try
                {
                    var prop = requestType.GetRuntimeProperties().FirstOrDefault(p =>
                        string.Equals(p.Name, rawProperty.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (prop == null)
                        throw new InvalidOperationException($"Raw request contains the unexpected field '{prop.Name}'");

                    prop.SetValue(request, rawProperty.ToObject(prop.PropertyType));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to deserialise '{rawProperty.Name}' for '{requestType}' - {ex.Message}");
                }
            }

            return request;
        }
    }
}