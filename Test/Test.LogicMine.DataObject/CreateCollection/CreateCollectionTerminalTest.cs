using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateCollection;
using LogicMine.DataObject.GetCollection;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.CreateCollection
{
    public class CreateCollectionTerminalTest : TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var store = GetStore(0);
            var terminal = new CreateCollectionTerminal<Frog<int>>(store);

            var frogs = new[]
            {
                new Frog<int> {Name = Guid.NewGuid().ToString(), DateOfBirth = DateTime.Today.AddDays(-8)},
                new Frog<int> {Name = Guid.NewGuid().ToString(), DateOfBirth = DateTime.Today.AddDays(-7)},
                new Frog<int> {Name = Guid.NewGuid().ToString(), DateOfBirth = DateTime.Today.AddDays(-6)},
            };

            var request = new CreateCollectionRequest<Frog<int>>(frogs);
            var basket = new Basket<CreateCollectionRequest<Frog<int>>, CreateCollectionResponse<Frog<int>>>(request);

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Success);
            Assert.Null(basket.Response.Error);
            Assert.True(basket.Response.Date < DateTime.Now && basket.Response.Date > DateTime.Now.AddSeconds(-5));
            Assert.False(string.IsNullOrEmpty(basket.Response.RequestId.ToString()));

            var readRequest = new GetCollectionRequest<Frog<int>>();
            var readBasket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(readRequest);
            var readTerminal = new GetCollectionTerminal<Frog<int>>(store);
            await readTerminal.AddResponseAsync(readBasket).ConfigureAwait(false);

            Assert.True(readBasket.Response.Objects.Length == 3);
            Assert.Contains(readBasket.Response.Objects, f => f.DateOfBirth == DateTime.Today.AddDays(-8));
            Assert.Contains(readBasket.Response.Objects, f => f.DateOfBirth == DateTime.Today.AddDays(-7));
            Assert.Contains(readBasket.Response.Objects, f => f.DateOfBirth == DateTime.Today.AddDays(-6));
        }
    }
}