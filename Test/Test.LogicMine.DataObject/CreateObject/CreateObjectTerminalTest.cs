using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.GetObject;
using Microsoft.Data.Sqlite;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.CreateObject
{
    public class CreateObjectTerminalTest : TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var store = GetStore(0);
            var terminal = new CreateObjectTerminal<Frog<int>, int>(store);

            var frog = new Frog<int>
                {Id = 1, Name = Guid.NewGuid().ToString(), DateOfBirth = DateTime.Today.AddDays(-8)};

            var request = new CreateObjectRequest<Frog<int>>(frog);
            var basket = new Basket<CreateObjectRequest<Frog<int>>, CreateObjectResponse<Frog<int>, int>>(request);

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.False(string.IsNullOrWhiteSpace(basket.Response.ObjectId.ToString()));
            Assert.Null(basket.Response.Error);
            Assert.True(basket.Response.Date < DateTime.Now && basket.Response.Date > DateTime.Now.AddSeconds(-5));
            Assert.False(string.IsNullOrEmpty(basket.Response.RequestId.ToString()));

            var id = basket.Response.ObjectId;

            var readRequest = new GetObjectRequest<Frog<int>, int>(id);
            var readBasket = new Basket<GetObjectRequest<Frog<int>, int>, GetObjectResponse<Frog<int>, int>>(readRequest);
            var readTerminal = new GetObjectTerminal<Frog<int>, int>(store);
            await readTerminal.AddResponseAsync(readBasket).ConfigureAwait(false);

            var readFrog = readBasket.Response.Object;
            Assert.Equal(frog, readFrog);
        }

        [Fact]
        public async Task AddResponseAsync_SpecificBasket()
        {
            var store = GetStore(0);
            var terminal = new CreateObjectTerminal<Frog<int>, int>(store);

            var frog = new Frog<int>
                {Id = 1, Name = Guid.NewGuid().ToString(), DateOfBirth = DateTime.Today.AddDays(-8)};

            var basket = new CreateObjectBasket<Frog<int>, int>(new CreateObjectRequest<Frog<int>>(frog));

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.False(string.IsNullOrWhiteSpace(basket.Response.ObjectId.ToString()));
            Assert.Null(basket.Response.Error);
            Assert.True(basket.Response.Date < DateTime.Now && basket.Response.Date > DateTime.Now.AddSeconds(-5));
            Assert.False(string.IsNullOrEmpty(basket.Response.RequestId.ToString()));

            var id = basket.Response.ObjectId;

            var readBasket = new GetObjectBasket<Frog<int>, int>(new GetObjectRequest<Frog<int>, int>(id));
            var readTerminal = new GetObjectTerminal<Frog<int>, int>(store);
            await readTerminal.AddResponseAsync(readBasket).ConfigureAwait(false);

            var readFrog = readBasket.Response.Object;
            Assert.Equal(frog, readFrog);
        }

        [Fact]
        public async Task AddResponseAsync_BadObject()
        {
            var store = GetStore(0);
            var terminal = new CreateObjectTerminal<Frog<int>, int>(store);

            var frog = new Frog<int>
                {DateOfBirth = DateTime.Today.AddDays(-8)};

            var request = new CreateObjectRequest<Frog<int>>(frog);
            var basket = new Basket<CreateObjectRequest<Frog<int>>, CreateObjectResponse<Frog<int>, int>>(request);

            var ex = await Assert.ThrowsAsync<SqliteException>(() => terminal.AddResponseAsync(basket))
                .ConfigureAwait(false);

            Assert.Contains("NOT NULL constraint failed: Frog.Name", ex.Message);
            Assert.Null(basket.Response);
        }
        
        [Fact]
        public async Task AddResponseAsync_NullBasket()
        {
            var store = GetStore(0);
            var terminal = new CreateObjectTerminal<Frog<int>, int>(store);

            await Assert.ThrowsAsync<ArgumentNullException>(() => terminal.AddResponseAsync(null))
                .ConfigureAwait(false);
        }
    }
}