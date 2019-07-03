using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LogicMine
{
    /// <inheritdoc cref="IBasket{TRequest,TResponse}" />
    public class Basket<TRequest, TResponse> : IBasket<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : IResponse
    {
        private readonly List<IVisit> _visits = new List<IVisit>();

        /// <inheritdoc />
        public DateTime StartedAt { get; private set; }

        /// <inheritdoc />
        public TimeSpan JourneyDuration { get; internal set; }

        /// <inheritdoc />
        public IVisit CurrentVisit { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IVisit> Visits => new ReadOnlyCollection<IVisit>(_visits);

        /// <inheritdoc />
        public bool IsFlaggedForRetrieval { get; internal set; }

        /// <inheritdoc />
        public TRequest Request { get; private set; }

        /// <inheritdoc />
        public TResponse Response { get; set; }

        /// <inheritdoc />
        IRequest IBasket.Request => Request;

        /// <inheritdoc />
        IResponse IBasket.Response
        {
            get => Response;
            set => Response = (TResponse) value;
        }

        /// <inheritdoc />
        public Exception Error { get; internal set; }

        internal static Basket<TRequest, TResponse> Copy(IBasket basket)
        {
            var destination = new Basket<TRequest, TResponse>();
            CopyOver(basket, destination);

            return destination;
        }

        internal static void CopyOver(IBasket source, Basket<TRequest, TResponse> destination)
        {
            destination._visits.AddRange(source.Visits);
            destination.CurrentVisit = source.CurrentVisit;
            destination.StartedAt = source.StartedAt;
            destination.JourneyDuration = source.JourneyDuration;
            destination.IsFlaggedForRetrieval = source.IsFlaggedForRetrieval;
            destination.Error = source.Error;

            if (!(source.Request is TRequest request))
            {
                throw new InvalidOperationException(
                    $"Expected the request to be a '{typeof(TRequest)}' but it was a '{source.Request?.GetType()}'");
            }

            if (!(source.Response is TResponse) && source.Response != null)
            {
                throw new InvalidOperationException(
                    $"Expected the response to be a '{typeof(TResponse)}' but it was a '{source.Response?.GetType()}'");
            }

            destination.Request = request;
            destination.Response = (TResponse) source.Response;
        }

        private Basket()
        {
        }

        /// <summary>
        /// Constructs a new basket
        /// </summary>
        /// <param name="request">The request that will travel within the basket</param>
        /// <exception cref="ArgumentNullException">Thrown if the request is null</exception>
        public Basket(TRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            StartedAt = DateTime.Now;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown if the visit argument is null</exception>
        public void AddVisit(IVisit visit)
        {
            CurrentVisit = visit ?? throw new ArgumentNullException(nameof(visit));
            _visits.Add(visit);
        }

        /// <inheritdoc />
        public void FlagForRetrieval()
        {
            IsFlaggedForRetrieval = true;
        }
    }
}