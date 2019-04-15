using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <summary>
    /// This represents a mine from the perspective of an object within the mine, i.e. shafts, stations and terminals.
    ///
    /// When an object within a mine needs to fork off a request down another shaft we want this new request to be
    /// associated with the original so that traces can be complete.
    /// </summary>
    public interface IContainingMine
    {
        /// <summary>
        /// Sends a request into the mine, the mine will choose the correct shaft to dispatch it to.
        /// </summary>
        /// <param name="parent">The basket which was being processed when this request was made</param>
        /// <param name="request">The request</param>
        /// <param name="inheritParentOptions">If true then the options within the parent request are copied into the new request</param>
        /// <returns>The response</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is an error processing the request</exception>
        Task<IResponse> SendAsync(IBasket parent, IRequest request, bool inheritParentOptions = true);

        /// <summary>
        /// Sends a request into the mine, the mine will choose the correct shaft to dispatch it to.
        /// </summary>
        /// <param name="parent">The basket which was being processed when this request was made</param>
        /// <param name="request">The request</param>
        /// <param name="inheritParentOptions">If true then the options within the parent request are copied into the new request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns>The response</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is an error processing the request</exception>
        Task<TResponse> SendAsync<TRequest, TResponse>(IBasket parent, TRequest request,
            bool inheritParentOptions = true)
            where TRequest : IRequest
            where TResponse : IResponse<TRequest>;

        /// <summary>
        /// Sends a basket into the mine, the mine will choose the correct shaft to dispatch it to.
        /// </summary>
        /// <param name="parent">The basket which was being processed when this request was made</param>
        /// <param name="basket">The basket that contains the new request</param>
        /// <param name="inheritParentOptions">If true then the options within the parent request are copied into the new request</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if there is an error processing the basket</exception>
        Task SendAsync(IBasket parent, IBasket basket, bool inheritParentOptions = true);
    }
}