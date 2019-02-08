using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LogicMine
{
    public class Basket<TRequest, TResponse> : IBasket<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        private readonly List<IVisit> _visits = new List<IVisit>();

        public DateTime StartedAt { get; }
        public TimeSpan JourneyDuration { get; set; }
        public IVisit CurrentVisit { get; private set; }
        public IReadOnlyCollection<IVisit> Visits => new ReadOnlyCollection<IVisit>(_visits);
        public bool IsFlagForRetrieval { get; set; }

        public IBasketPayload<TRequest, TResponse> Payload { get; }
        IBasketPayload IBasket.Payload => Payload;

        public Exception Error { get; set; }

        public Basket(IBasketPayload<TRequest, TResponse> payload)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            StartedAt = DateTime.Now;
        }

        public void AddVisit(IVisit visit)
        {
            CurrentVisit = visit ?? throw new ArgumentNullException(nameof(visit));
            _visits.Add(visit);
        }
    }

//    public class Basket : IBasket
//    {
//        public IBasketPayload Payload { get; }
//
//        public Basket(IBasketPayload payload)
//        {
//            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
//        }
//    }
}