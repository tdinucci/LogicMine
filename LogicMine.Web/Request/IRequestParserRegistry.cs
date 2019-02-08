namespace LogicMine.Web.Request
{
    public interface IRequestParserRegistry<TRawRequest>
    {
        IRequestParserRegistry<TRawRequest> Register(IRequestParser<TRawRequest> parser);

        IRequestParser<TRawRequest> Get(TRawRequest request);
    }
}