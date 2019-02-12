using System;
using LogicMine.DataObject;
using LogicMine.DataObject.DeleteObject;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json.DataObject
{
    /// <summary>
    /// A parser which specialises in parsing DeleteObjectRequests from JObjects
    /// </summary>
    public class DeleteObjectRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        public DeleteObjectRequestJsonParser(
            IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("deleteObject");
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

            var requestType = typeof(DeleteObjectRequest<,>).MakeGenericType(descriptor.DataType, descriptor.IdType);
            return (IRequest) Activator.CreateInstance(requestType, id);
        }
    }
}