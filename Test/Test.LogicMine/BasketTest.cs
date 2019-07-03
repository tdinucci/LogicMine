using System;
using System.Collections.Generic;
using System.Linq;
using LogicMine;
using Test.Common.LogicMine.Mine.GetTime;
using Xunit;

namespace Test.LogicMine
{
    public class BasketTest
    {
        [Fact]
        public void Construct()
        {
            var request = new GetTimeRequest();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(request);

            Assert.True(basket.StartedAt < DateTime.Now && basket.StartedAt > DateTime.Now.AddSeconds(-1));
            Assert.Equal(TimeSpan.Zero, basket.JourneyDuration);
            Assert.True(basket.Visits != null && !basket.Visits.Any());
            Assert.Null(basket.CurrentVisit);
            Assert.Null(basket.Error);
            Assert.Equal(request, basket.Request);
            Assert.False(basket.IsFlaggedForRetrieval);
        }

        [Fact]
        public void Construct_BadArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new Basket<GetTimeRequest, GetTimeResponse>(null));
        }

        [Fact]
        public void AddVisit()
        {
            var request = new GetTimeRequest();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(request);

            var visits = new List<IVisit>();
            for (var i = 0; i < 50; i++)
            {
                var visit = new Visit(i.ToString(), VisitDirection.Down);
                basket.AddVisit(visit);

                Assert.Equal(visit, basket.CurrentVisit);

                visits.Add(visit);
            }

            Assert.True(basket.Visits.Count == visits.Count);
            Assert.Equal(visits.Last(), basket.CurrentVisit);

            for (var i = 0; i < visits.Count; i++)
                Assert.Equal(visits[i], basket.Visits.ElementAt(i));
        }

        [Fact]
        public void AddVisit_BadArgs()
        {
            var request = new GetTimeRequest();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(request);

            Assert.Throws<ArgumentNullException>(() => basket.AddVisit(null));
        }
    }
}