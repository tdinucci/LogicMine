using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine.DataObject;

namespace LogicMine.Routing
{
    /// <inheritdoc />
    public abstract class RequestRouter<TRawRequest> : IRequestRouter<TRawRequest>
    {
        private bool _isInitialised;
        private readonly IMine _mine;
        private readonly IErrorExporter _errorExporter;

        /// <summary>
        /// The registry of parsers which can be used to parse requests
        /// </summary>
        protected IRequestParserRegistry<TRawRequest> ParserRegistry { get; private set; }
        
        /// <summary>
        /// The registry of data object descriptors 
        /// </summary>
        protected IDataObjectDescriptorRegistry DataObjectDescriptorRegistry { get; private set; }

        /// <summary>
        /// Construct a new RequestRouter
        /// </summary>
        /// <param name="mine">The mine to route requests to</param>
        /// <param name="errorExporter">The exporter to use when errors are encountered</param>
        protected RequestRouter(IMine mine, IErrorExporter errorExporter)
        {
            _mine = mine ?? throw new ArgumentNullException(nameof(mine));
            _errorExporter = errorExporter;
        }

        /// <summary>
        /// Returns the data object descriptors which are available to the service
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IDataObjectDescriptor> GetDataObjectDescriptors();
        
        /// <summary>
        /// Returns the shaft registrars which are available to the service
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IShaftRegistrar> GetShaftRegistrars();
        
        /// <summary>
        /// Returns the parser registry
        /// </summary>
        /// <returns></returns>
        protected abstract IRequestParserRegistry<TRawRequest> GetParserRegistry();

        /// <summary>
        /// Initialises the request router, putting it into a state so that it can process requests
        /// </summary>
        protected virtual void Initialise()
        {
            if (!_isInitialised)
            {
                DataObjectDescriptorRegistry = GetDescriptorRegistry();
                ParserRegistry = GetParserRegistry();
                InitialiseMineShafts(_mine);

                _isInitialised = true;
            }
        }

        /// <summary>
        /// Called after a request has been parsed but before it has been routed.  This gives you the opportunity
        /// to modify the request before it enters the mine.  For example, you may extract authorisation information
        /// from HTTP request headers and add these to the request.
        /// </summary>
        /// <param name="request"></param>
        protected virtual void PreprocessRequest(IRequest request)
        {
        }

        /// <summary>
        /// Generates a descriptor registry which contains the descriptors gained from calling GetDataObjectDescriptors()
        /// </summary>
        /// <returns></returns>
        protected virtual IDataObjectDescriptorRegistry GetDescriptorRegistry()
        {
            var registry = new DataObjectDescriptorRegistry();
            foreach (var descriptor in GetDataObjectDescriptors())
                registry.Register(descriptor);

            return registry;
        }

        /// <summary>
        /// Initialises the mine using the shaft registrars obtained by calling GetShaftRegistrars()
        /// </summary>
        /// <param name="mine"></param>
        protected virtual void InitialiseMineShafts(IMine mine)
        {
            foreach (var shaftRegistrar in GetShaftRegistrars())
                shaftRegistrar.RegisterShafts(mine);
        }

        /// <inheritdoc />
        public async Task<IResponse> RouteAsync(TRawRequest rawRequest)
        {
            if (!_isInitialised)
                Initialise();

            IRequest parsedRequest = null;
            try
            {
                if (rawRequest == null) throw new ArgumentNullException(nameof(rawRequest));

                parsedRequest = ParserRegistry.Get(rawRequest).Parse(rawRequest);
                PreprocessRequest(parsedRequest);

                return await _mine.SendAsync(parsedRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _errorExporter?.ExportError(ex);

                if (ex.InnerException != null)
                    ex = ex.InnerException;

                return new Response(parsedRequest, ex.Message);
            }
        }
    }
}