using System;
using System.Collections.Generic;
using System.Linq;
using LogicMine.DataObject.UpdateObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectRequestTest
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

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Equal(id, request.ObjectId);
            Assert.Equal(props, request.ModifiedProperties);
        }

        [Fact]
        public void Construct_BadArgs()
        {
            var ex = Assert.Throws<ArgumentException>(() => new UpdateObjectRequest<Frog<int>, int>(5, null));
            Assert.Equal("'modifiedProperties' must have a value", ex.Message);
        }
    }
}