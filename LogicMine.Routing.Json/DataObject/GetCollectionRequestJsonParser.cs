using System;
using LogicMine.DataObject;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.Filter.Parse;
using LogicMine.DataObject.GetCollection;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json.DataObject
{
    public class GetCollectionRequestJsonParser : JsonRequestParser
    {
        private readonly IDataObjectDescriptorRegistry _dataObjectDescriptor;

        public GetCollectionRequestJsonParser(IDataObjectDescriptorRegistry dataObjectDescriptorRegistry)
        {
            _dataObjectDescriptor =
                dataObjectDescriptorRegistry ?? throw new ArgumentNullException(nameof(dataObjectDescriptorRegistry));

            AddHandledRequestType("getCollection");
        }

        public override IRequest Parse(JObject rawRequest)
        {
            EnsureCanHandleRequest(rawRequest);

            if (!rawRequest.ContainsKey("type"))
                throw new InvalidOperationException("Request does not specify a data type");
            
            var dataTypeName = rawRequest["type"].Value<string>();
            var descriptor = _dataObjectDescriptor.GetDescriptor(dataTypeName);

            IFilter filter = null;
            int? max = null;
            int? page = null;

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

            if (rawRequest.ContainsKey("max"))
                max = rawRequest["max"].Value<int?>();

            if (rawRequest.ContainsKey("page"))
                page = rawRequest["page"].Value<int?>();

            var requestType = typeof(GetCollectionRequest<>).MakeGenericType(descriptor.DataType);
            return (IRequest) Activator.CreateInstance(requestType, filter, max, page);
        }
    }
}