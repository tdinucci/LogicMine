namespace LogicMine
{
    /// <inheritdoc />
    public abstract class ShaftRegistrar : IShaftRegistrar
    {
        /// <inheritdoc />
        public abstract void RegisterShafts(IMine mine);

        /// <summary>
        /// Returns a standard shaft.  For instance, if each shaft within a mine should include a station that enforces
        /// security then this method should return a shaft which includes the security enforcing station.
        /// </summary>
        /// <param name="terminal">The shaft terminal</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        protected abstract IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
            where TRequest : class, IRequest
            where TResponse : IResponse;
    }
}