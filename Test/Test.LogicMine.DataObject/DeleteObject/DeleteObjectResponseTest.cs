using System;
using LogicMine.DataObject.DeleteObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectResponseTest
    {
        [Fact]
        public void Construct()
        {
            var id = DateTime.Now.Millisecond;
            var request = new DeleteObjectRequest<Frog<int>, int>(id);
            var response = new DeleteObjectResponse<Frog<int>, int>(request);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.False(response.Success);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }

        [Fact]
        public void Construct_Success()
        {
            var id = DateTime.Now.Millisecond;
            var request = new DeleteObjectRequest<Frog<int>, int>(id);
            var response = new DeleteObjectResponse<Frog<int>, int>(request, true);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.True(response.Success);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }

        [Fact]
        public void Construct_Error()
        {
            var error = Guid.NewGuid().ToString();
            var id = DateTime.Now.Millisecond;
            var request = new DeleteObjectRequest<Frog<int>, int>(id);
            var response = new DeleteObjectResponse<Frog<int>, int>(request, false, error);

            Assert.Equal(error, response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.False(response.Success);
        }
    }
}