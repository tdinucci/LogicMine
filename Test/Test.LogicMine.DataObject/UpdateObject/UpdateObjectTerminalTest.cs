using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.UpdateObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectTerminalTest : TerminalTest
    {
        [Fact]
        public async Task AddResponseAsync()
        {
            var store = GetStore(10);
            var terminal = new UpdateObjectTerminal<Frog<int>, int>(store);

            var id = 5;
            var newName = Guid.NewGuid().ToString();
            var newDob = DateTime.Now.AddDays(-15);
            var modifiedProperties = new Dictionary<string, object>
            {
                {nameof(Frog<int>.Name), newName},
                {nameof(Frog<int>.DateOfBirth), newDob},
            };
            
            var request = new UpdateObjectRequest<Frog<int>, int>(id, modifiedProperties);
            var basket = new Basket<UpdateObjectRequest<Frog<int>, int>, UpdateObjectResponse>(request);

            await terminal.AddResponseAsync(basket).ConfigureAwait(false);

            Assert.True(basket.Response.Success);
            Assert.Null(basket.Response.Error);
            Assert.True(basket.Response.Date < DateTime.Now && basket.Response.Date > DateTime.Now.AddSeconds(-5));
            Assert.False(string.IsNullOrEmpty(basket.Response.RequestId.ToString()));

            var readRequest = new GetObjectRequest<Frog<int>, int>(id);
            var readBasket = new Basket<GetObjectRequest<Frog<int>, int>, GetObjectResponse<Frog<int>>>(readRequest);
            var readTerminal = new GetObjectTerminal<Frog<int>, int>(store);
            await readTerminal.AddResponseAsync(readBasket).ConfigureAwait(false);

            Assert.Equal(id, readBasket.Response.Object.Id);
            Assert.Equal(newName, readBasket.Response.Object.Name);
            Assert.Equal(newDob, readBasket.Response.Object.DateOfBirth);
        }
        
        [Fact]
        public async Task AddResponseAsync_NullBasket()
        {
            var store = GetStore(0);
            var terminal = new UpdateObjectTerminal<Frog<int>, int>(store);

            await Assert.ThrowsAsync<ArgumentNullException>(() => terminal.AddResponseAsync(null))
                .ConfigureAwait(false);
        }
    }
}