using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.GetCollection;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.GetCollection
{
    public class GetCollectionTerminalTest : TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var store = GetStore(10);
            
            var request = new GetCollectionRequest<Frog<int>>();
            var basket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(request);
            var terminal = new GetCollectionTerminal<Frog<int>>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.Equal(10, basket.Response.Objects.Length);
        }
        
        [Fact]
        public async Task AddResponseAsync_GoodFilter()
        {
            var store = GetStore(10);

            var filter =
                new Filter<Frog<int>>(new[] {new FilterTerm(nameof(Frog<int>.Id), FilterOperators.LessThan, 5)});
            
            var request = new GetCollectionRequest<Frog<int>>(filter);
            var basket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(request);
            var terminal = new GetCollectionTerminal<Frog<int>>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.Equal(4, basket.Response.Objects.Length);
        }

        [Fact]
        public async Task AddResponseAsync_BadFilter()
        {
            var store = GetStore(10);

            var filter =
                new Filter<Frog<int>>(new[] {new FilterTerm(nameof(Frog<int>.Id), FilterOperators.GreaterThan, 100)});

            var request = new GetCollectionRequest<Frog<int>>(filter);
            var basket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(request);
            var terminal = new GetCollectionTerminal<Frog<int>>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Objects.Length == 0);
        }
        
        [Fact]
        public async Task AddResponseAsync_Max()
        {
            var store = GetStore(10);

            var request = new GetCollectionRequest<Frog<int>>(3, 0);
            var basket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(request);
            var terminal = new GetCollectionTerminal<Frog<int>>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Objects.Length == 3);
        }

        [Fact]
        public async Task AddResponseAsync_Paged()
        {
            var store = GetStore(10);

            var request = new GetCollectionRequest<Frog<int>>(4, 1);
            var basket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(request);
            var terminal = new GetCollectionTerminal<Frog<int>>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Objects.Length == 4);
            Assert.Equal(5, basket.Response.Objects[0].Id);
        }
        
        [Fact]
        public async Task AddResponseAsync_FilteredPaged()
        {
            var store = GetStore(10);

            var filter =
                new Filter<Frog<int>>(new[] {new FilterTerm(nameof(Frog<int>.Id), FilterOperators.LessThanOrEqual, 5)});
            
            var request = new GetCollectionRequest<Frog<int>>(filter, 3, 1, null);
            var basket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(request);
            var terminal = new GetCollectionTerminal<Frog<int>>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Objects.Length == 2); // should only be 2 results on 2nd page
            Assert.Equal(5, basket.Response.Objects[1].Id);
        }

        [Fact]
        public async Task AddResponseAsync_NullBasket()
        {
            var store = GetStore(10);

            var terminal = new GetCollectionTerminal<Frog<int>>(store);

            await Assert.ThrowsAsync<ArgumentNullException>(() => terminal.AddResponseAsync(null)).ConfigureAwait(false);
        }
    }
}