using System;
using System.Collections.Generic;
using System.Linq;
using LogicMine.Routing.Json.DataObject;
using Newtonsoft.Json.Linq;

namespace LogicMine.Routing.Json
{
    /// <summary>
    /// A <see cref="RequestRouter{TRawRequest}"/> that specialises in routing JObject requests
    /// </summary>
    public abstract class JsonRequestRouter : RequestRouter<JObject>
    {
        protected JsonRequestRouter(IMine mine, IErrorExporter errorExporter) : base(mine, errorExporter)
        {
        }

        protected abstract IEnumerable<Type> GetCustomRequestTypes();

        protected override IRequestParserRegistry<JObject> GetParserRegistry()
        {
            var customRequestTypes = GetCustomRequestTypes()?.ToArray() ?? new Type[0];

            return new JsonRequestParserRegistry()
                .Register(new NonGenericJsonRequestParser(customRequestTypes))
                .Register(new GetObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new GetCollectionRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new CreateObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new UpdateObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new DeleteObjectRequestJsonParser(DataObjectDescriptorRegistry));
        }
    }
}