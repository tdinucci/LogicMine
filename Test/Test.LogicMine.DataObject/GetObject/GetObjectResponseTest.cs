using System;
using LogicMine.DataObject.GetObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.GetObject
{
    public class GetObjectResponseTest
    {
        [Fact]
        public void Construct()
        {
            var id = DateTime.Now.Millisecond;
            var frog = new Frog<int> {Id = id};
            var request = new GetObjectRequest<Frog<int>, int>(id);
            var response = new GetObjectResponse<Frog<int>, int>(request, frog);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal(frog, response.Object);
            Assert.Equal(typeof(Frog<int>).Name, response.ObjectType);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }

        [Fact]
        public void Construct_NoObject()
        {
            var id = DateTime.Now.Millisecond;
            var request = new GetObjectRequest<Frog<int>, int>(id);
            var response = new GetObjectResponse<Frog<int>, int>(request);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Null(response.Object);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }
    }
}