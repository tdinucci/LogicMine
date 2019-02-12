using System;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateObject;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json.DataObject
{
    /// <summary>
    /// A parser which specialises in parsing CreateObjectRequests from JObjects
    /// </summary>
    public class CreateObjectRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        /// <summary>
        /// Construct a CreateObjectRequestJsonParser
        /// </summary>
        /// <param name="dataObjectDescriptorRegistry">The registry of data object descriptors</param>
        public CreateObjectRequestJsonParser(
            IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("createObject");
        }

        /// <inheritdoc />
        public override IRequest Parse(JObject rawRequest)
        {
            EnsureCanHandleRequest(rawRequest);

            if (!rawRequest.ContainsKey("type"))
                throw new InvalidOperationException("Request does not specify a data type");
            if (!rawRequest.ContainsKey("object"))
                throw new InvalidOperationException("Request does not specify an object");

            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);
            var obj = rawRequest["object"].ToObject(descriptor.DataType);

            var requestType = typeof(CreateObjectRequest<>).MakeGenericType(descriptor.DataType);
            return (IRequest) Activator.CreateInstance(requestType, obj);
        }
    }
}