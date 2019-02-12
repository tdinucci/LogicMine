using System;
using System.Collections.Generic;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.UpdateObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectResponseTest
    {
        [Fact]
        public void Construct()
        {
            var id = DateTime.Now.Millisecond;
            var props = new Dictionary<string, object>
            {
                {nameof(Frog<int>.Name), "NewName"},
                {nameof(Frog<int>.DateOfBirth), DateTime.Now},
            };
            var request = new UpdateObjectRequest<Frog<int>, int>(id, props);
            var response = new UpdateObjectResponse(request, true);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.True(response.Success);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }

        [Fact]
        public void Construct_Fail()
        {
            var id = DateTime.Now.Millisecond;
            var props = new Dictionary<string, object>
            {
                {nameof(Frog<int>.Name), "NewName"},
                {nameof(Frog<int>.DateOfBirth), DateTime.Now},
            };
            var request = new UpdateObjectRequest<Frog<int>, int>(id, props);
            var response = new UpdateObjectResponse(request);

            Assert.Null(response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.False(response.Success);

            var error = Guid.NewGuid().ToString();
            response.Error = error;
            Assert.Equal(error, response.Error);
        }
        
        [Fact]
        public void Construct_Error()
        {
            var id = DateTime.Now.Millisecond;
            var error = Guid.NewGuid().ToString();
            var props = new Dictionary<string, object>
            {
                {nameof(Frog<int>.Name), "NewName"},
                {nameof(Frog<int>.DateOfBirth), DateTime.Now},
            };
            var request = new UpdateObjectRequest<Frog<int>, int>(id, props);
            var response = new UpdateObjectResponse(request, false, error);

            Assert.Equal(error, response.Error);
            Assert.Equal(request.Id, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.False(response.Success);
        }
    }
}