using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine
{
    public abstract class DefaultShaftRegistrar : ShaftRegistrar
    {
        private readonly ITraceExporter _traceExporter;

        protected DefaultShaftRegistrar(ITraceExporter traceExporter)
        {
            _traceExporter = traceExporter ?? throw new ArgumentNullException(nameof(traceExporter));
        }

        protected IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal) 
            where TRequest : class, IRequest 
            where TResponse : IResponse
        {
            return new Shaft<TRequest, TResponse>(_traceExporter, terminal, new SecurityStation());
        }
    }
}