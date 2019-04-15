using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using Microsoft.Data.Sqlite;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectTerminalTest : TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var store = GetStore(10);
            var terminal = new DeleteObjectTerminal<Frog<int>, int>(store);

            var id = 5;
            var request = new DeleteObjectRequest<Frog<int>, int>(id);
            var basket = new Basket<DeleteObjectRequest<Frog<int>, int>, DeleteObjectResponse<Frog<int>, int>>(request);

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Success);
            Assert.Null(basket.Response.Error);
            Assert.True(basket.Response.Date < DateTime.Now && basket.Response.Date > DateTime.Now.AddSeconds(-5));
            Assert.False(string.IsNullOrEmpty(basket.Response.RequestId.ToString()));

            var readRequest = new GetCollectionRequest<Frog<int>>();
            var readBasket = new Basket<GetCollectionRequest<Frog<int>>, GetCollectionResponse<Frog<int>>>(readRequest);
            var readTerminal = new GetCollectionTerminal<Frog<int>>(store);
            await readTerminal.AddResponseAsync(readBasket).ConfigureAwait(false);

            Assert.Equal(9, readBasket.Response.Objects.Length);
            Assert.True(readBasket.Response.Objects.FirstOrDefault(f => f.Id == id) == null);
        }
        
        [Fact]
        public async Task AddResponseAsync_NullBasket()
        {
            var store = GetStore(0);
            var terminal = new DeleteObjectTerminal<Frog<int>, int>(store);

            await Assert.ThrowsAsync<ArgumentNullException>(() => terminal.AddResponseAsync(null))
                .ConfigureAwait(false);
        }
    }
}