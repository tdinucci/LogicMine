using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <summary>
    /// An internal interface which allows for certain properties to be manipulated from types within this library
    /// </summary>
    internal interface IInternalShaft : IShaft
    {
        /// <summary>
        /// A reference to the mine which contains the shaft
        /// </summary>
        new IContainingMine Within { get; set; }
    }

    /// <inheritdoc />
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface IShaft<in TRequest, TResponse> : IShaft
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <summary>
        /// Add stations to the top of the shaft
        /// </summary>
        /// <param name="stations">The stations to add</param>
        /// <returns>A reference to the current shaft</returns>
        new IShaft<TRequest, TResponse> AddToTop(params IStation[] stations);
        
        /// <summary>
        /// Add stations to the bottom of the shaft (but above the terminal)
        /// </summary>
        /// <param name="stations">The stations to add</param>
        /// <returns>A reference to the current shaft</returns>
        new IShaft<TRequest, TResponse> AddToBottom(params IStation[] stations);
        
        /// <summary>
        /// Send a request down the shaft
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>The response from the shaft</returns>
        Task<TResponse> SendAsync(TRequest request);
    }

    /// <summary>
    /// A shaft is an abstraction of an execution pipeline.  A request is sent into the shaft which consists of a single
    /// terminal and one or more stations.  The request flows down through the stations until it hits the terminal (where
    /// a response will typically be generated) at which point the request and response flow back up through the shafts.
    ///
    /// Requests and responses are wrapped within a basket - <see cref="IBasket"/> 
    /// </summary>
    public interface IShaft
    {
        /// <summary>
        /// A reference to the mine which contains the shaft
        /// </summary>
        IContainingMine Within { get; }
        
        /// <summary>
        /// The request type
        /// </summary>
        Type RequestType { get; }
        
        /// <summary>
        /// The expected response type
        /// </summary>
        Type ResponseType { get; }

        /// <summary>
        /// If true then the shaft will execute within a transaction
        /// </summary>
        bool ExecuteWithinTransaction { get; set; }

        /// <summary>
        /// Add stations to the top of the shaft
        /// </summary>
        /// <param name="stations">The stations to add</param>
        /// <returns>A reference to the current shaft</returns>
        IShaft AddToTop(params IStation[] stations);
        
        /// <summary>
        /// Add stations to the bottom of the shaft (but above the terminal)
        /// </summary>
        /// <param name="stations">The stations to add</param>
        /// <returns>A reference to the current shaft</returns>
        IShaft AddToBottom(params IStation[] stations);

        /// <summary>
        /// Send a request down the shaft
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>The response from the shaft</returns>
        Task<IResponse> SendAsync(IRequest request);
    }
}