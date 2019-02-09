using System;
using LogicMine.DataObject;
using LogicMine.DataObject.GetObject;
using Newtonsoft.Json.Linq;

namespace LogicMine.Web.Request
{
    public class GetObjectRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;
        public override string HandledRequestType { get; } = "getObject";

        public GetObjectRequestJsonParser(IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));
        }

        public override IRequest Parse(JObject rawRequest)
        {
            if (!CanHandleRequest(rawRequest))
            {
                throw new InvalidOperationException(
                    $"This parser handles '{HandledRequestType}' not '{GetRequestType(rawRequest)}'");
            }

            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);
            var id = rawRequest["id"].ToObject(descriptor.IdType);

            var requestType = typeof(GetObjectRequest<,>).MakeGenericType(descriptor.DataType, id.GetType());
            return (IRequest) Activator.CreateInstance(requestType, id);
        }
    }
}