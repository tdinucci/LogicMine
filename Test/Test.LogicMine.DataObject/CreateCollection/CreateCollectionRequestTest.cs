using System;
using System.Linq;
using LogicMine.DataObject.CreateCollection;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.CreateCollection
{
    public class CreateCollectionRequestTest
    {
        [Fact]
        public void Construct()
        {
            var frogs = new[]
            {
                new Frog<int>(),
                new Frog<int>(),
                new Frog<int>()

            };
            var request = new CreateCollectionRequest<Frog<int>>(frogs);

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.True(request.Objects.Count() == 3);
        }

        [Fact]
        public void Construct_BadArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateCollectionRequest<Frog<int>>(null));
        }
    }
}