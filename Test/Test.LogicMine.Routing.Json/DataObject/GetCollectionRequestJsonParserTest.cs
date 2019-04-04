using System.Linq;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.Routing.Json.DataObject;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Test.LogicMine.Routing.Json.DataObject
{
    public class GetCollectionRequestJsonParserTest
    {
        private readonly Helper _helper = new Helper();

        [Fact]
        public void TypeOnly()
        {
            var rawRequest = new JObject
            {
                {"requestType", "getCollection"},
                {"type", "intFrog"}
            };

            var request = (GetCollectionRequest<IntFrog>)
                new GetCollectionRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);

            Assert.True(request.ObjectType == typeof(IntFrog));
            Assert.Null(request.Filter);
            Assert.Null(request.Select);
        }

        [Fact]
        public void Filter()
        {
            var rawRequest = new JObject
            {
                {"requestType", "getCollection"},
                {"type", "intFrog"},
                {"filter", "id gt 5 and name eq kermit"}
            };

            var request = (GetCollectionRequest<IntFrog>)
                new GetCollectionRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);

            Assert.True(request.ObjectType == typeof(IntFrog));

            Assert.True(request.Filter.Terms.Count() == 2);
            Assert.Contains(5, request.Filter.Terms
                .Where(t => t.PropertyName == "Id" && t.Operator == FilterOperators.GreaterThan)
                .Select(t => t.Value));

            Assert.Null(request.Select);
        }

        [Fact]
        public void SelectList()
        {
            var rawRequest = new JObject
            {
                {"requestType", "getCollection"},
                {"type", "intFrog"},
                {"select", JArray.FromObject(new[] {"name", "dateOfBirth"})}
            };

            var request = (GetCollectionRequest<IntFrog>)
                new GetCollectionRequestJsonParser(_helper.GetRegistry()).Parse(rawRequest);

            Assert.True(request.ObjectType == typeof(IntFrog));

            Assert.True(request.Select.Length == 2);
            Assert.Contains("name", request.Select);
            Assert.Contains("dateOfBirth", request.Select);
        }
    }
}