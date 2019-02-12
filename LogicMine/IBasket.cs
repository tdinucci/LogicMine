using System;
using System.Collections.Generic;

namespace LogicMine
{   
    /// <inheritdoc />
    /// <typeparam name="TRequest">The type of request contained in the basket</typeparam>
    /// <typeparam name="TResponse">The type of response contained in the basket</typeparam>
    public interface IBasket<out TRequest, TResponse> : IBasket
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        /// <summary>
        /// The request that is travelling within the basket
        /// </summary>
        new TRequest Request { get; }
        
        /// <summary>
        /// The response that is travelling within the basket
        /// </summary>
        new TResponse Response { get; set; }
    }

    /// <summary>
    /// A basket is fundamentally a container for requests and responses.  In addition however they can 
    /// record the journey the basket takes and also enable communication with the objects which encounter
    /// the basket.
    /// </summary>
    public interface IBasket
    {
        /// <summary>
        /// The time the basket started it's journey at
        /// </summary>
        DateTime StartedAt { get; }

        /// <summary>
        /// The amount of time the basket has travelled for
        /// </summary>
        TimeSpan JourneyDuration { get; }

        /// <summary>
        /// The current/last visit made by the basket
        /// </summary>
        IVisit CurrentVisit { get; }

        /// <summary>
        /// The visits which have been made by the basket
        /// </summary>
        IReadOnlyCollection<IVisit> Visits { get; }

        /// <summary>
        /// If true then the basket should be pulled up it's shaft immediately
        /// </summary>
        bool IsFlagForRetrieval { get; }

        /// <summary>
        /// The request that is travelling within the basket
        /// </summary>
        IRequest Request { get; }
        
        /// <summary>
        /// The response that is travelling within the basket
        /// </summary>
        IResponse Response { get; set; }

        /// <summary>
        /// Any error that occurred while processing the basket.  Normally this will be null
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Adds a visit to the basket 
        /// </summary>
        /// <param name="visit">The visit to add</param>
        void AddVisit(IVisit visit);
    }
}