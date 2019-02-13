using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc />
    public class Mine : IMine
    {
        private readonly ITraceExporter _traceExporter;
        private readonly Dictionary<Type, IShaft> _requestTypeShafts = new Dictionary<Type, IShaft>();

        /// <summary>
        /// Constructs a mine
        /// </summary>
        /// <param name="traceExporter">A trace exporter</param>
        public Mine(ITraceExporter traceExporter = null)
        {
            _traceExporter = traceExporter;
        }

        /// <inheritdoc />
        public IMine AddShaft(IShaft shaft)
        {
            if (shaft == null) throw new ArgumentNullException(nameof(shaft));

            if (_requestTypeShafts.ContainsKey(shaft.RequestType))
            {
                throw new InvalidOperationException(
                    $"There is already a shaft registered for request type '{shaft.RequestType}'");
            }

            _requestTypeShafts.Add(shaft.RequestType, shaft);
            if (shaft is IInternalShaft internalShaft)
                internalShaft.Within = this;

            return this;
        }

        /// <inheritdoc />
        public async Task<IResponse> SendAsync(IRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var requestType = request.GetType();
                if (!_requestTypeShafts.ContainsKey(requestType))
                {
                    throw new InvalidOperationException(
                        $"There are no shafts registered to handle request type '{requestType}'");
                }

                return await _requestTypeShafts[requestType].SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _traceExporter?.ExportError(ex);

                return new Response(request, ex.Message);
            }
        }

        /// <inheritdoc />
        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest
            where TResponse : IResponse
        {
            try
            {
                var response = await SendAsync(request).ConfigureAwait(false);
                if (response != null && !(response is TResponse))
                {
                    throw new InvalidOperationException(
                        $"Expected response to be a '{typeof(TResponse)}' but it was a '{response.GetType()}'");
                }

                return (TResponse) response;
            }
            catch (Exception ex)
            {
                _traceExporter?.ExportError(ex);

                return ResponseFactory.Create<TResponse>(request, ex.Message);
            }
        }
    }
}