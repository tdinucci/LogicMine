using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine
{
    /// This is what we'll base all non-data-object shaft registrars on.  Basically it's just ensuring that each of 
    /// our shafts contain a SecurityStation at the top - so we don't risk forgetting to add in security.
    ///
    /// Non-data-object shafts can be used to process requests of any type - in fact data object shafts are build on
    /// top of them.
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
            where TResponse : IResponse<TRequest>
        {
            return new Shaft<TRequest, TResponse>(_traceExporter, terminal, new SecurityStation());
        }
    }
}