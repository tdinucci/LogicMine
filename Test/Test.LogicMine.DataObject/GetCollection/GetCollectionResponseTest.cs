using System;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.GetCollection;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.GetCollection
{
    public class GetCollectionResponseTest
    {
        [Fact]
        public void Construct()
        {
            var request = new GetCollectionRequest<Frog<int>>();
            var response = new GetCollectionResponse<Frog<int>>(request);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Null(response.Objects);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }

        [Fact]
        public void Construct_WithResults()
        {
            var request = new GetCollectionRequest<Frog<int>>();
            var results = new[] {new Frog<int>(), new Frog<int>(), new Frog<int>()};
            var response = new GetCollectionResponse<Frog<int>>(request, results);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal(results, response.Objects);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }
    }
}