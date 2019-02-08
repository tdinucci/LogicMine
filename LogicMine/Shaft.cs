using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogicMine
{
    public class Shaft<TRequest, TResponse> : IShaft<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse, new()
    {
        private readonly IList<IStation> _stations = new List<IStation>();
        private readonly ITerminal _terminal;
        private readonly ITraceExporter _traceExporter;

        Type IShaft.RequestType { get; } = typeof(TRequest);
        Type IShaft.ResponseType { get; } = typeof(TResponse);

        public Shaft(ITerminal<TRequest, TResponse> terminal, params IStation[] stations) :
            this(null, terminal, stations)
        {
        }

        public Shaft(ITraceExporter traceExporter, ITerminal<TRequest, TResponse> terminal,
            params IStation[] stations)
        {
            _traceExporter = traceExporter;
            _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

            AddToBottom(stations.ToArray());
        }

        public void AddToTop(params IStation[] stations)
        {
            if (stations != null)
            {
                foreach (var station in stations.Reverse())
                {
                    EnsureCompatible(station);
                    _stations.Insert(0, station);
                }
            }
        }

        public void AddToBottom(params IStation[] stations)
        {
            if (stations != null)
            {
                foreach (var station in stations)
                {
                    EnsureCompatible(station);
                    _stations.Add(station);
                }
            }
        }

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

                return new TResponse {Error = ex.Message};
            }
        }

        public async Task<TResponse> SendAsync(TRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            Basket<TRequest, TResponse> basket = null;
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var payload = new BasketPayload<TRequest, TResponse>(request);
                basket = new Basket<TRequest, TResponse>(payload);

                await DescendAsync(basket).ConfigureAwait(false);
                if (basket.IsFlagForRetrieval)
                    return basket.Payload.Response;

                await VisitTerminal(basket).ConfigureAwait(false);
                if (basket.IsFlagForRetrieval)
                    return basket.Payload.Response;

                await AscendAsync(basket).ConfigureAwait(false);

                return basket.Payload.Response;
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
                
                return new TResponse {Error = ex.Message};
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

        private async Task DescendAsync(IBasket basket)
        {
            var stopwatch = new Stopwatch();
            foreach (var station in _stations)
            {
                var visit = new Visit(station.ToString(), VisitDirection.Down);
                basket.AddVisit(visit);
                
                try
                {
                    stopwatch.Start();

                    await station.DescendToAsync(basket).ConfigureAwait(false);

                    stopwatch.Stop();
                    visit.Duration = stopwatch.Elapsed;
                    stopwatch.Reset();

                    if (basket.IsFlagForRetrieval)
                        return;


                }
                catch (Exception ex)
                {
                    visit.Exception = ex;
                    throw;
                }
            }
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

        private async Task AscendAsync(IBasket basket)
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

                    await station.AscendFromAsync(basket).ConfigureAwait(false);

                    stopwatch.Stop();
                    visit.Duration = stopwatch.Elapsed;
                    stopwatch.Reset();

                    if (basket.IsFlagForRetrieval)
                        return;
                }
                catch (Exception ex)
                {
                    visit.Exception = ex;
                    throw;
                }
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

//    public class Shaft : IShaft
//    {
//        protected readonly IList<IStation> Stations = new List<IStation>();
//        protected readonly ITerminal Terminal;
//
//        public Type RequestType { get; }
//        public Type ResponseType { get; }
//
//        public Shaft(Type requestType, Type responseType, ITerminal terminal, params IStation[] stations)
//        {
//            RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
//            ResponseType = responseType ?? throw new ArgumentNullException(nameof(responseType));
//            Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
//
//            AddToBottom(stations);
//        }
//
//        public void AddToTop(params IStation[] stations)
//        {
//            if (stations != null)
//            {
//                foreach (var station in stations.Reverse())
//                {
//                    EnsureCompatible(station);
//                    Stations.Insert(0, station);
//                }
//            }
//        }
//
//        public void AddToBottom(params IStation[] stations)
//        {
//            if (stations != null)
//            {
//                foreach (var station in stations)
//                {
//                    EnsureCompatible(station);
//                    Stations.Add(station);
//                }
//            }
//        }
//
//        public async Task<IResponse> SendAsync(IRequest request)
//        {
//            var payload = new BasketPayload(request, ResponseType);
//            var basket = new Basket(payload);
//
//            await DescendAsync(basket).ConfigureAwait(false);
//            await VisitTerminal(basket).ConfigureAwait(false);
//            await AscendAsync(basket).ConfigureAwait(false);
//
//            return basket.Payload.Response;
//        }
//
//        private async Task DescendAsync(IBasket basket)
//        {
//            foreach (var station in Stations)
//            {
//                await station.DescendToAsync(basket).ConfigureAwait(false);
//            }
//        }
//
//        private async Task VisitTerminal(IBasket basket)
//        {
//            await Terminal.AddResponseAsync(basket).ConfigureAwait(false);
//        }
//
//        private async Task AscendAsync(IBasket basket)
//        {
//            for (var i = Stations.Count - 1; i >= 0; i--)
//            {
//                await Stations[i].AscendFromAsync(basket).ConfigureAwait(false);
//            }
//        }
//
//        private void EnsureCompatible(IStation station)
//        {
//            if (!station.RequestType.IsAssignableFrom(RequestType))
//            {
//                throw new InvalidOperationException(
//                    $"The station request type of '{station.RequestType}' is not compatible with the shafts request type of '{RequestType}'");
//            }
//
//            if (!station.ResponseType.IsAssignableFrom(ResponseType))
//            {
//                throw new InvalidOperationException(
//                    $"The station response type of '{station.ResponseType}' is not compatible with the shafts response type of '{ResponseType}'");
//            }
//        }
//    }
}