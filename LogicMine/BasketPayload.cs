using System;

namespace LogicMine
{
    public class BasketPayload<TRequest, TResponse> : BasketPayload, IBasketPayload<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        public new TRequest Request => (TRequest) base.Request;

        public new TResponse Response
        {
            get => (TResponse) base.Response;
            set => base.Response = value;
        }

        public BasketPayload(TRequest request) : base(request, typeof(TResponse))
        {
        }
    }

    public class BasketPayload : IBasketPayload
    {
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public IRequest Request { get; }
        public IResponse Response { get; set; }

        public BasketPayload(IRequest request, Type responseType)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            ResponseType = responseType ?? throw new ArgumentNullException(nameof(responseType));

            RequestType = request.GetType();
        }

        public IBasketPayload<TRequest, TResponse> Unwrap<TRequest, TResponse>()
            where TRequest : class, IRequest
            where TResponse : IResponse
        {
            if (!(Request is TRequest))
            {
                throw new InvalidOperationException(
                    $"The request type '{Request.GetType()}' is not a '{typeof(TRequest)}'");
            }

            if (Response != null && !(Response is TResponse))
            {
                throw new InvalidOperationException(
                    $"The response type '{Response.GetType()}' is not a '{typeof(TResponse)}'");
            }

            var unwrapped = new BasketPayload<TRequest, TResponse>((TRequest) Request);
            if (Response != null)
                unwrapped.Response = (TResponse) Response;

            return unwrapped;
        }
    }
}