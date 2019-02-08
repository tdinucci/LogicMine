using System;
using System.Collections.Generic;

namespace LogicMine
{
    public interface IBasket<out TRequest, TResponse> : IBasket
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        new IBasketPayload<TRequest, TResponse> Payload { get; }
    }

    public interface IBasket
    {
        /// <summary>
        /// The time the basket started it's journey at
        /// </summary>
        DateTime StartedAt { get; }
    
        /// <summary>
        /// The amount of time the basket has travelled for
        /// </summary>
        TimeSpan JourneyDuration { get; set; }
        
        /// <summary>
        /// The current/last visit made by the basket
        /// </summary>
        IVisit CurrentVisit { get; }
        
        /// <summary>
        /// The visits which have been made by the basket
        /// </summary>
        IReadOnlyCollection<IVisit> Visits { get; }
        
        /// <summary>
        /// Set to true to indicate that the basket should be pulled up it's shaft immediately
        /// </summary>
        bool IsFlagForRetrieval { get; set; }
        
        IBasketPayload Payload { get; }

        Exception Error { get; set; }
        
        void AddVisit(IVisit visit);
    }
}