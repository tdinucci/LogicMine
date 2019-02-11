using System;
using System.Threading.Tasks;
using LogicMine;
using Test.Common.LogicMine.Mine.GetDeconstructedDate;
using Test.Common.LogicMine.Mine.GetTime;
using Xunit;

namespace Test.LogicMine
{
    public class TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var request = new GetTimeRequest();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(request);

            var terminal = new GetTimeTerminal();

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            var time = basket.Response.Time;
            Assert.True(time < DateTime.Now && time > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public async Task AddResponseAsync_NonGeneric()
        {
            var request = new GetTimeRequest();
            var basket = new Basket<GetTimeRequest, GetTimeResponse>(request);

            var terminal = new GetTimeTerminal() as ITerminal;

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            var time = basket.Response.Time;
            Assert.True(time < DateTime.Now && time > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public async Task AddResponseAsync_NonGenericTerminal_BadBasket()
        {
            var request = new GetDeconstructedDateRequest();
            var basket = new Basket<GetDeconstructedDateRequest, GetDeconstructedDateRespone>(request);

            var terminal = new GetTimeTerminal() as ITerminal;

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => terminal.AddResponseAsync(basket))
                .ConfigureAwait(false);

            Assert.Equal(
                $"Expected basket t be a '{typeof(IBasket<GetTimeRequest, GetTimeResponse>)}' but it was a '{basket.GetType()}'",
                ex.Message);
        }
    }
}