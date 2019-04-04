using System;
using System.Linq;
using LogicMine.DataObject;
using LogicMine.DataObject.GetObject;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json.DataObject
{
    /// <summary>
    /// A parser which specialises in parsing GetObjectRequests from JObjects
    /// </summary>
    public class GetObjectRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        public GetObjectRequestJsonParser(IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("getObject");
        }

        /// <inheritdoc />
        public override IRequest Parse(JObject rawRequest)
        {
            EnsureCanHandleRequest(rawRequest);

            if (!rawRequest.ContainsKey("type"))
                throw new InvalidOperationException("Request does not specify a data type");

            if (!rawRequest.ContainsKey("id"))
                throw new InvalidOperationException("Request does not specify an Id");

            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);
            var id = rawRequest["id"].ToObject(descriptor.IdType);

            string[] select = null;
            if (rawRequest.ContainsKey("select"))
                select = rawRequest["select"].Values<string>().ToArray();

            var requestType = typeof(GetObjectRequest<,>).MakeGenericType(descriptor.DataType, id.GetType());
            return (IRequest) Activator.CreateInstance(requestType, id, select);
        }
    }
}