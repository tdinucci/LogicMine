using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <inheritdoc />
    public abstract class Terminal<TRequest, TResponse> : ITerminal<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <inheritdoc />
        public abstract Task AddResponseAsync(IBasket<TRequest, TResponse> basket);

        /// <inheritdoc />
        Task ITerminal.AddResponseAsync(IBasket basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));

            if (!(basket is IBasket<TRequest, TResponse> castBasket))
            {
                throw new InvalidOperationException(
                    $"Expected basket t be a '{typeof(IBasket<TRequest, TResponse>)}' but it was a '{basket.GetType()}'");
            }

            return AddResponseAsync(castBasket);
        }
    }
}