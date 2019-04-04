using System.Linq;
using LogicMine.DataObject.DeleteCollection;
using LogicMine.DataObject.Filter;
using LogicMine.Routing.Json.DataObject;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Test.LogicMine.Routing.Json.DataObject
{
    public class DeleteCollectionRequestJsonParserTest
    {
        private readonly Helper _helper = new Helper();

        [Fact]
        public void Test()
        {
            var rawRequest = new JObject
            {
                {"requestType", "deleteCollection"},
                {"type", "intFrog"},
                {"filter", "id gt 5 and name eq kermit"}
            };

            var request = (DeleteCollectionRequest<IntFrog>)
                new DeleteCollectionRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);

            Assert.True(request.ObjectType == typeof(IntFrog));
            Assert.True(request.Filter.Terms.Count() == 2);
            Assert.Contains(5, request.Filter.Terms
                .Where(t => t.PropertyName == "Id" && t.Operator == FilterOperators.GreaterThan)
                .Select(t => t.Value));
        }
    }
}