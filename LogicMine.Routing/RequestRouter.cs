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

        protected IRequestParserRegistry<TRawRequest> ParserRegistry { get; private set; }
        protected IDataObjectDescriptorRegistry DataObjectDescriptorRegistry { get; private set; }

        protected RequestRouter(IMine mine, IErrorExporter errorExporter)
        {
            _mine = mine ?? throw new ArgumentNullException(nameof(mine));
            _errorExporter = errorExporter;
        }

        protected abstract IEnumerable<IDataObjectDescriptor> GetDataObjectDescriptors();
        protected abstract IEnumerable<IShaftRegistrar> GetShaftRegistrars();
        protected abstract IRequestParserRegistry<TRawRequest> GetParserRegistry();

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

        protected virtual void PreprocessRequest(IRequest request)
        {
        }

        protected virtual IDataObjectDescriptorRegistry GetDescriptorRegistry()
        {
            var registry = new DataObjectDescriptorRegistry();
            foreach (var descriptor in GetDataObjectDescriptors())
                registry.Register(descriptor);

            return registry;
        }

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