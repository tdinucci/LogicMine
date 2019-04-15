using System;

namespace LogicMine
{
    /// <summary>
    /// A factory for IResponse objects
    /// </summary>
    public static class ResponseFactory
    {
        /// <summary>
        /// Create a new TResponse based on the given IRequest
        /// </summary>
        /// <param name="request">The request which lead to the response</param>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns>A response based on the given request</returns>
        /// <exception cref="InvalidOperationException">Thrown if the TResponse does not have a constructor which accepts only an IRequest</exception>
        public static TResponse Create<TResponse>(IRequest request) where TResponse : IResponse
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var ctor = typeof(TResponse).GetConstructor(new[] {request.GetType()});
            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"There is no ctor on '{typeof(TResponse)}' that accepts only a '{request.GetType().Name}'");
            }

            return (TResponse) ctor.Invoke(new object[] {request});
        }

        /// <summary>
        /// Create a new error TResponse based on the given IRequest
        /// </summary>
        /// <param name="request">The request which lead to the response</param>
        /// <param name="error">The error description</param>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns>An error response based on the given request</returns>
        /// <exception cref="InvalidOperationException">Thrown if the TResponse does not have a constructor which accepts only an IRequest</exception>
        public static TResponse Create<TResponse>(IRequest request, string error) where TResponse : IResponse
        {
            var response = Create<TResponse>(request);
            response.Error = error;

            return response;
        }
    }
}