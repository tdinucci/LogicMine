using LogicMine.DataObject.CreateCollection;
using LogicMine.Routing.Json.DataObject;
using Newtonsoft.Json.Linq;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.Routing.Json.DataObject
{
    public class CreateCollectionRequestJsonParserTest
    {
        private readonly Helper _helper = new Helper();

        [Fact]
        public void Test()
        {
            var rawRequest = new JObject
            {
                {"requestType", "createCollection"},
                {"type", "intFrog"},
                {"objects", JArray.FromObject(new[]
                {
                    new Frog<int>(),
                    new Frog<int>(),
                    new Frog<int>()
                })}
            };

            var request = new CreateCollectionRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);
            Assert.IsType(typeof(CreateCollectionRequest<Frog<int>>), request);
        }
    }
}