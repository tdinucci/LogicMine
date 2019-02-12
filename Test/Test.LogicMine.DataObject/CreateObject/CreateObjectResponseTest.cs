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
            var request = new CreateObjectRequest<Frog<int>>(frog);
            var response = new CreateObjectResponse<Frog<int>, int>(request, 88);

            Assert.Equal(request.Id, response.RequestId);
            Assert.Equal(88, response.ObjectId);
        }
    }
}