using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.GetObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.GetObject
{
    public class GetObjectTerminalTest : TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var store = GetStore(10);
            var id = 6;

            var request = new GetObjectRequest<Frog<int>, int>(id);
            var basket = new Basket<GetObjectRequest<Frog<int>, int>, GetObjectResponse<Frog<int>, int>>(request);
            var terminal = new GetObjectTerminal<Frog<int>, int>(store);
            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            var frog = basket.Response.Object;
            Assert.Null(basket.Response.Error);
            Assert.Equal(typeof(Frog<int>).Name, basket.Response.ObjectType);
            Assert.Equal(id, frog.Id);
        }

        [Fact]
        public async Task AddResponseAsync_BadId()
        {
            var store = GetStore(0);
            var id = 600;

            var request = new GetObjectRequest<Frog<int>, int>(id);
            var basket = new Basket<GetObjectRequest<Frog<int>, int>, GetObjectResponse<Frog<int>, int>>(request);
            var terminal = new GetObjectTerminal<Frog<int>, int>(store);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(()=>terminal.AddResponseAsync(basket)).ConfigureAwait(false);

            Assert.Equal($"No '{typeof(Frog<int>)}' record found", ex.Message);
        }
        
        [Fact]
        public async Task AddResponseAsync_NullBasket()
        {
            var store = GetStore(0);
            var terminal = new GetObjectTerminal<Frog<int>, int>(store);

            await Assert.ThrowsAsync<ArgumentNullException>(() => terminal.AddResponseAsync(null))
                .ConfigureAwait(false);
        }
    }
}