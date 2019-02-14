using System;
using LogicMine;
using LogicMine.DataObject;

namespace Sample.LogicMine.Shop.Service.Mine
{
    /// <summary>
    /// This is what we'll base all data object shaft registrars on.  Basically it's just ensuring that each of 
    /// our shafts contain a SecurityStation at the top - so we don't risk forgetting to add in security.
    ///
    /// Data object shafts are shafts which contain terminals which handle CRUD like operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
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