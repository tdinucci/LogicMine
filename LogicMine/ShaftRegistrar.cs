namespace LogicMine
{
    public abstract class ShaftRegistrar : IShaftRegistrar
    {
        public abstract void RegisterShafts(IMine mine);

        protected abstract IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
            where TRequest : class, IRequest
            where TResponse : IResponse, new();
    }
}