using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LogicMine.Trace
{
    public abstract class LoggerBasket
    {
        private readonly IBasket _basket;
        private const string InfoStatus = "Info";
        private const string ErrorStatus = "Error";

        private List<LoggerVisit> _visits = new List<LoggerVisit>();

        // internal setters so that tests can modify these things and so that 
        // we can get to them with serialisers
        public string Service { get; internal set; }
        public string RequestId { get; internal set; }
        public string ParentRequestId { get; internal set; }
        public string RequestType { get; internal set; }
        public string Request { get; internal set; }
        public string Response { get; internal set; }
        public string Status { get; internal set; }
        public DateTime StartedAt { get; internal set; }
        public TimeSpan Duration { get; internal set; }

        public IEnumerable<LoggerVisit> Visits
        {
            get => new ReadOnlyCollection<LoggerVisit>(_visits);
            internal set => _visits = value?.ToList();
        }

        internal LoggerBasket()
        {
        }

        protected LoggerBasket(string service, IBasket basket)
        {
            Service = service;

            if (basket?.Request == null)
            {
                RequestType = "UNKNOWN";
                Status = ErrorStatus;
                // this will create a visit which contains an error
                _visits.Add(new LoggerVisit(null));
            }
            else
            {
                _basket = basket;
                RequestId = basket.Request.Id.ToString();
                ParentRequestId = basket.Request.ParentId?.ToString();
                RequestType = basket.Request.GetType().Name;
                Status = basket.Error != null ? ErrorStatus : InfoStatus;
                StartedAt = basket.StartedAt;
                Duration = basket.JourneyDuration;
                if (_basket.Visits != null)
                    AddVisits(_basket.Visits);
            }
        }

        protected abstract string SerialiseObject(object obj);

        private void AddVisits(IEnumerable<IVisit> visits)
        {
            if (visits != null)
            {
                foreach (var visit in visits)
                {
                    _visits.Add(new LoggerVisit(visit));
                    if (visit.Exception != null)
                        Status = ErrorStatus;
                }
            }
        }

        public virtual string Serialise()
        {
            if (_basket != null)
            {
                Request = SerialiseObject(_basket.Request);
                Response = SerialiseObject(_basket.Response);
            }

            return SerialiseObject(this);
        }
    }
}