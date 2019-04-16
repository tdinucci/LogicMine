using LogicMine;

namespace Security.Mine
{
    public class MyShaft<TRequest, TResponse> : Shaft<TRequest, TResponse> 
        where TRequest : class, IRequest
        where TResponse : IResponse<TRequest>
    {
        public MyShaft(ITerminal<TRequest, TResponse> terminal, params IStation[] stations) : base(terminal, stations)
        {
            AddToTop(new SecurityStation());
        }
    }
}