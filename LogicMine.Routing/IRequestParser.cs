using System.Collections.Immutable;

namespace LogicMine.Routing
{
    public interface IRequestParser<in TRawRequest>
    {
        ImmutableHashSet<string> HandledRequestTypes { get; }

        bool CanHandleRequest(TRawRequest rawRequest);
        void EnsureCanHandleRequest(TRawRequest rawRequest);

        IRequest Parse(TRawRequest rawRequest);
        TRequest Parse<TRequest>(TRawRequest rawRequest);
    }
}