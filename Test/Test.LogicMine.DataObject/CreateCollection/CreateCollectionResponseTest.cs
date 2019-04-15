using System;
using LogicMine.DataObject.CreateCollection;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.CreateCollection
{
    public class CreateCollectionResponseTest
    {
        [Fact]
        public void Construct()
        {
            var frogs = new[]
            {
                new Frog<int>(),
                new Frog<int>(),
                new Frog<int>()

            };
            
            var request = new CreateCollectionRequest<Frog<int>>(frogs);
            var response = new CreateCollectionResponse<Frog<int>>(request, true);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.True(response.Success);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }
    }
}