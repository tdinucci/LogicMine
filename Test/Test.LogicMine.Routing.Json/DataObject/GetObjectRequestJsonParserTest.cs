using LogicMine.DataObject.GetObject;
using LogicMine.Routing.Json.DataObject;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Test.LogicMine.Routing.Json.DataObject
{
    public class GetObjectRequestJsonParserTest
    {
        private readonly Helper _helper = new Helper();

        [Fact]
        public void IdOnly()
        {
            var rawRequest = new JObject
            {
                {"requestType", "getObject"},
                {"type", "intFrog"},
                {"id", 987}
            };

            var request = (GetObjectRequest<IntFrog, int>)
                new GetObjectRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);

            Assert.True(request.ObjectType == typeof(IntFrog));
            Assert.Equal(987, request.ObjectId);
            Assert.Null(request.Select);
        }

        [Fact]
        public void SelectList()
        {
            var rawRequest = new JObject
            {
                {"requestType", "getObject"},
                {"type", "intFrog"},
                {"id", 987},
                {"select", JArray.FromObject(new[] {"name", "dateOfBirth"})}
            };

            var request = (GetObjectRequest<IntFrog, int>)
                new GetObjectRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);

            Assert.True(request.ObjectType == typeof(IntFrog));
            Assert.Equal(987, request.ObjectId);

            Assert.True(request.Select.Length == 2);
            Assert.Contains("name", request.Select);
            Assert.Contains("dateOfBirth", request.Select);
        }
    }
}