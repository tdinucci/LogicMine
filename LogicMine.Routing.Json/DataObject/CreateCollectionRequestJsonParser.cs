using System;
using System.Linq;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateCollection;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json.DataObject
{
    /// <summary>
    /// A parser which specialises in parsing CreateCollectionRequests from JObjects
    /// </summary>
    public class CreateCollectionRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        /// <summary>
        /// Construct a CreateCollectionRequestJsonParser
        /// </summary>
        /// <param name="dataObjectDescriptorRegistry">The registry of data object descriptors</param>
        public CreateCollectionRequestJsonParser(
            IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("createCollection");
        }

        /// <inheritdoc />
        public override IRequest Parse(JObject rawRequest)
        {
            EnsureCanHandleRequest(rawRequest);

            if (!rawRequest.ContainsKey("type"))
                throw new InvalidOperationException("Request does not specify a data type");
            if (!rawRequest.ContainsKey("objects"))
                throw new InvalidOperationException("Request does not specify any objects");

            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);
            var objs = rawRequest["objects"].Select(o => o.ToObject(descriptor.DataType)).ToArray();
            
            // objs will be an array of object, we need an array of descriptor.DataType
            var objArray = Array.CreateInstance(descriptor.DataType, objs.Length);
            Array.Copy(objs, objArray, objs.Length);

            var requestType = typeof(CreateCollectionRequest<>).MakeGenericType(descriptor.DataType);
            return (IRequest) Activator.CreateInstance(requestType, objArray);
        }
    }
}