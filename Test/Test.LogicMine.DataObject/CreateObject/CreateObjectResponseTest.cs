using System;
using LogicMine.DataObject.CreateObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.CreateObject
{
    public class CreateObjectResponseTest
    {
        [Fact]
        public void Construct()
        {
            var frog = new Frog<int>();
            var id = DateTime.Now.Millisecond;
            var request = new CreateObjectRequest<Frog<int>>(frog);
            var response = new CreateObjectResponse<Frog<int>, int>(request, id);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal(id, response.ObjectId);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }

        [Fact]
        public void Construct_NoId()
        {
            var frog = new Frog<int>();
            var request = new CreateObjectRequest<Frog<int>>(frog);
            var response = new CreateObjectResponse<Frog<int>, int>(request);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal(0, response.ObjectId);
        }
    }
}