using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc />
    public class Shaft<TRequest, TResponse> : IShaft<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        private readonly IList<IStation> _stations = new List<IStation>();
        private readonly ITerminal _terminal;
        private readonly ITraceExporter _traceExporter;

        /// <inheritdoc />
        Type IShaft.RequestType { get; } = typeof(TRequest);

        /// <inheritdoc />
        Type IShaft.ResponseType { get; } = typeof(TResponse);

        /// <summary>
        /// Constructs a new shaft
        /// </summary>
        /// <param name="terminal">The terminal of the shaft</param>
        /// <param name="stations">The stations above the terminal</param>
        public Shaft(ITerminal<TRequest, TResponse> terminal, params IStation[] stations) :
            this(null, terminal, stations)
        {
        }

        /// <summary>
        /// Constructs a new shaft
        /// </summary>
        /// <param name="traceExporter">A trace exporter</param>
        /// <param name="terminal">The terminal of the shaft</param>
        /// <param name="stations">The stations above the terminal</param>
        public Shaft(ITraceExporter traceExporter, ITerminal<TRequest, TResponse> terminal,
            params IStation[] stations)
        {
            _traceExporter = traceExporter;
            _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

            AddToBottom(stations.ToArray());
        }

        /// <inheritdoc />
        IShaft IShaft.AddToTop(params IStation[] stations)
        {
            return AddToTop(stations);
        }

        /// <inheritdoc />
        IShaft IShaft.AddToBottom(params IStation[] stations)
        {
            return AddToBottom(stations);
        }

        /// <inheritdoc />
        public IShaft<TRequest, TResponse> AddToTop(params IStation[] stations)
        {
            if (stations != null)
            {
                foreach (var station in stations.Reverse())
                {
                    EnsureCompatible(station);
                    _stations.Insert(0, station);
                }
            }

            return this;
        }

        /// <inheritdoc />
        public IShaft<TRequest, TResponse> AddToBottom(params IStation[] stations)
        {
            if (stations != null)
            {
                foreach (var station in stations)
                {
                    EnsureCompatible(station);
                    _stations.Add(station);
                }
            }

            return this;
        }

        /// <inheritdoc />
        async Task<IResponse> IShaft.SendAsync(IRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                if (!(request is TRequest))
                {
                    throw new InvalidOperationException(
                        $"Expected request to be a '{typeof(TRequest)}' but it was a '{request.GetType()}'");
                }

                return await SendAsync((TRequest) request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _traceExporter?.ExportError(ex);

                return ResponseFactory.Create<TResponse>(request, ex.Message);
            }
        }

        /// <inheritdoc />
        public async Task<TResponse> SendAsync(TRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            Basket<TRequest, TResponse> basket = null;
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                basket = new Basket<TRequest, TResponse>(request);

                await DescendAsync(ref basket).ConfigureAwait(false);
                if (basket.IsFlagForRetrieval)
                    return basket.Response;

                await VisitTerminal(basket).ConfigureAwait(false);
                if (basket.IsFlagForRetrieval)
                    return basket.Response;

                await AscendAsync(ref basket).ConfigureAwait(false);

                return basket.Response;
            }
            catch (Exception ex)
            {
                if (basket != null)
                {
                    basket.Error = new ShaftException(
                        $"An error occurred processing the '{basket.GetType()}'.  See inner exception for details", ex);
                }
                else
                    _traceExporter.ExportError(ex);

                return ResponseFactory.Create<TResponse>(request, ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                if (basket != null)
                {
                    basket.JourneyDuration = stopwatch.Elapsed;
                    _traceExporter?.Export(basket);
                }
            }
        }

        private Task DescendAsync(ref Basket<TRequest, TResponse> basket)
        {
            var stopwatch = new Stopwatch();
            foreach (var station in _stations)
            {
                var visit = new Visit(station.ToString(), VisitDirection.Down);
                basket.AddVisit(visit);

                try
                {
                    stopwatch.Start();

                    var basketRef = basket as IBasket;
                    station.DescendToAsync(ref basketRef).GetAwaiter().GetResult();

                    stopwatch.Stop();
                    visit.Duration = stopwatch.Elapsed;
                    stopwatch.Reset();

                    if (basket.IsFlagForRetrieval)
                        return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    visit.Exception = ex;
                    throw;
                }
            }

            return Task.CompletedTask;
        }

        private Task AscendAsync(ref Basket<TRequest, TResponse> basket)
        {
            var stopwatch = new Stopwatch();
            for (var i = _stations.Count - 1; i >= 0; i--)
            {
                var station = _stations[i];
                var visit = new Visit(station.ToString(), VisitDirection.Up);
                basket.AddVisit(visit);

                try
                {
                    stopwatch.Start();

                    var basketRef = basket as IBasket;
                    station.AscendFromAsync(ref basketRef).GetAwaiter().GetResult();

                    stopwatch.Stop();
                    visit.Duration = stopwatch.Elapsed;
                    stopwatch.Reset();

                    if (basket.IsFlagForRetrieval)
                        return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    visit.Exception = ex;
                    throw;
                }
            }

            return Task.CompletedTask;
        }

        private async Task VisitTerminal(IBasket basket)
        {
            var visit = new Visit(_terminal.ToString(), VisitDirection.Down);
            basket.AddVisit(visit);

            try
            {
                var stopwatch = Stopwatch.StartNew();

                await _terminal.AddResponseAsync(basket).ConfigureAwait(false);

                stopwatch.Stop();
                visit.Duration = stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                visit.Exception = ex;
                throw;
            }
        }

        private void EnsureCompatible(IStation station)
        {
            var requestType = typeof(TRequest);
            var responseType = typeof(TResponse);

            if (!station.RequestType.IsAssignableFrom(requestType))
            {
                throw new InvalidOperationException(
                    $"The station request type of '{station.RequestType}' is not compatible with the shafts request type of '{requestType}'");
            }

            if (!station.ResponseType.IsAssignableFrom(responseType))
            {
                throw new InvalidOperationException(
                    $"The station response type of '{station.ResponseType}' is not compatible with the shafts response type of '{responseType}'");
            }
        }
    }
}