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
        /// <summary>
        /// Construct a JsonRequestRouter
        /// </summary>
        /// <param name="mine">The mine to route requests to</param>
        /// <param name="errorExporter">The error exporter to use when errors are encountered</param>
        protected JsonRequestRouter(IMine mine, IErrorExporter errorExporter = null) : base(mine, errorExporter)
        {
        }

        /// <summary>
        /// Returns the collection of custom requests which the router is aware of
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Type> GetCustomRequestTypes();

        /// <summary>
        /// Returns a parser registry which contains the standard compliment of JSON request parsers.
        /// </summary>
        /// <returns></returns>
        protected override IRequestParserRegistry<JObject> GetParserRegistry()
        {
            var customRequestTypes = GetCustomRequestTypes()?.ToArray() ?? new Type[0];

            return new JsonRequestParserRegistry()
                .Register(new NonGenericJsonRequestParser(customRequestTypes))
                .Register(new GetObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new GetCollectionRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new CreateObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new UpdateObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new DeleteObjectRequestJsonParser(DataObjectDescriptorRegistry))
                .Register(new DeleteCollectionRequestJsonParser(DataObjectDescriptorRegistry));
        }
    }
}