namespace LogicMine.Web.Request
{
    public interface IRequestParser<in TRawRequest> : IRequestParser
    {
        bool CanHandleRequest(TRawRequest rawRequest);
        IRequest Parse(TRawRequest rawRequest);
        TRequest Parse<TRequest>(TRawRequest rawRequest);
    }

    public interface IRequestParser
    {
        string HandledRequestType { get; }
    }
}