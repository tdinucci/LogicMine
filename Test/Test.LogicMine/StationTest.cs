using System;
using System.Threading.Tasks;
using LogicMine;
using Test.Common.LogicMine.Mine.GetTime;
using Xunit;

namespace Test.LogicMine
{
    public class StationTest
    {
        private const string AscendOption = "Hello from the way up";

        [Fact]
        public void Construct()
        {
            var station = new TestStation();
            Assert.Equal(typeof(GetTimeRequest), station.RequestType);
            Assert.Equal(typeof(GetTimeResponse), station.ResponseType);
        }

        [Fact]
        public async Task DescendToAsync()
        {
            var station = new TestStation();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(new GetTimeRequest());

            await station.DescendToAsync(basket).ConfigureAwait(false);
            Assert.True(basket.Response.Time < DateTime.Now && basket.Response.Time > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public async Task DescendToAsync_NonGeneric()
        {
            var station = new TestStation() as IStation;
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(new GetTimeRequest()) as IBasket;

            await station.DescendToAsync(ref basket).ConfigureAwait(false);

            var castBasket = basket as IBasket<GetTimeRequest, GetTimeResponse>;
            Assert.True(castBasket.Response.Time < DateTime.Now &&
                        castBasket.Response.Time > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public async Task AscendFromAsync()
        {
            var station = new TestStation();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(new GetTimeRequest());

            await station.AscendFromAsync(basket).ConfigureAwait(false);
            Assert.True(basket.Request.Options["opt1"].ToString() == AscendOption);
        }

        private class TestStation : Station<GetTimeRequest, GetTimeResponse>
        {
            public override Task DescendToAsync(IBasket<GetTimeRequest, GetTimeResponse> basket)
            {
                basket.Response = new GetTimeResponse(basket.Request, DateTime.Now);

                return Task.CompletedTask;
            }

            public override Task AscendFromAsync(IBasket<GetTimeRequest, GetTimeResponse> basket)
            {
                basket.Request.Options.Add("opt1", AscendOption);
                return Task.CompletedTask;
            }
        }
    }
}