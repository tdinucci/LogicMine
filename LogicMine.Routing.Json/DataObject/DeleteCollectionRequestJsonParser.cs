using System;
using LogicMine.DataObject;
using LogicMine.DataObject.DeleteCollection;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.Filter.Parse;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json.DataObject
{
    /// <summary>
    /// A parser which specialises in parsing DeleteCollectionRequests from JObjects
    /// </summary>
    public class DeleteCollectionRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        /// <summary>
        /// Construct a DeleteCollectionRequestJsonParser
        /// </summary>
        /// <param name="dataObjectDescriptorRegistry">The registry of data object descriptors</param>
        public DeleteCollectionRequestJsonParser(IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("deleteCollection");
        }

        /// <inheritdoc />
        public override IRequest Parse(JObject rawRequest)
        {
            EnsureCanHandleRequest(rawRequest);

            if (!rawRequest.ContainsKey("type"))
                throw new InvalidOperationException("Request does not specify a data type");
            if (!rawRequest.ContainsKey("filter"))
                throw new InvalidOperationException("Request does not specify a filter");

            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);

            IFilter filter = null;
            if (rawRequest.ContainsKey("filter"))
            {
                var filterString = rawRequest["filter"].Value<string>();
                if (!string.IsNullOrWhiteSpace(filterString))
                {
                    var filterParserType = typeof(FilterParser<>).MakeGenericType(descriptor.DataType);

                    var filterParser = (IFilterParser) Activator.CreateInstance(filterParserType, filterString);
                    filter = filterParser.Parse();
                }
            }

            var requestType = typeof(DeleteCollectionRequest<>).MakeGenericType(descriptor.DataType, descriptor.IdType);
            return (IRequest) Activator.CreateInstance(requestType, filter);
        }
    }
}