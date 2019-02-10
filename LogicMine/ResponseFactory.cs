using System;

namespace LogicMine
{
    public static class ResponseFactory
    {
        public static TResponse Create<TResponse>(IRequest request) where TResponse : IResponse
        {
            var ctor = typeof(TResponse).GetConstructor(new[] {typeof(IRequest)});
            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"There is no ctor on '{typeof(TResponse)}' that accepts only an '{typeof(IRequest).Name}'");
            }

            return (TResponse) ctor.Invoke(new object[] {request});
        }

        public static TResponse Create<TResponse>(IRequest request, string error) where TResponse : IResponse
        {
            var response = Create<TResponse>(request);
            response.Error = error;

            return response;
        }
    }
}