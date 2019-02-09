using System;
using System.Collections.Generic;
using LogicMine.DataObject;
using LogicMine.DataObject.UpdateObject;
using Newtonsoft.Json.Linq;

namespace LogicMine.Web.Request.Json.DataObject
{
    public class UpdateObjectRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        public UpdateObjectRequestJsonParser(
            IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("updateObject");
        }

        public override IRequest Parse(JObject rawRequest)
        {
            EnsureCanHandleRequest(rawRequest);

            if (!rawRequest.ContainsKey("type"))
                throw new InvalidOperationException("Request does not specify a data type");
            if (!rawRequest.ContainsKey("id"))
                throw new InvalidOperationException("Request does not specify an Id");
            if (!rawRequest.ContainsKey("modifiedProperties"))
                throw new InvalidOperationException("Request does not specify the modified properties");

            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);
            var id = rawRequest["id"].ToObject(descriptor.IdType);
            var modifiedProperties = rawRequest["modifiedProperties"].ToObject<Dictionary<string, object>>();

            var requestType = typeof(UpdateObjectRequest<,>).MakeGenericType(descriptor.DataType, descriptor.IdType);
            return (IRequest) Activator.CreateInstance(requestType, id, modifiedProperties);
        }
    }
}