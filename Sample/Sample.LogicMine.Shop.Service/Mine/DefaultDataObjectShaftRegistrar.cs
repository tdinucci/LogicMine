using System;
using LogicMine;
using LogicMine.DataObject;

namespace Sample.LogicMine.Shop.Service.Mine
{
    public abstract class DefaultDataObjectShaftRegistrar<T, TId> : DataObjectShaftRegistrar<T, TId>
        where T : class, new()
    {
        private readonly ITraceExporter _traceExporter;

        protected DefaultDataObjectShaftRegistrar(ITraceExporter traceExporter)
        {
            _traceExporter = traceExporter ?? throw new ArgumentNullException(nameof(traceExporter));
        }

        protected override IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
        {
            return new Shaft<TRequest, TResponse>(_traceExporter, terminal, new SecurityStation());
        }
    }
}