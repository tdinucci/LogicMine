using System;
using System.Linq;
using LogicMine.DataObject.GetObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.GetObject
{
    public class GetObjectRequestTest
    {
        [Fact]
        public void Construct()
        {
            var id = DateTime.Now.Millisecond;
            var request = new GetObjectRequest<Frog<int>, int>(id);

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Equal(id, request.ObjectId);
        }
    }
}