using System;
using LogicMine;
using Xunit;

namespace Test.LogicMine
{
    public class RequestTest
    {
        [Fact]
        public void Construct()
        {
            var request = new TestRequest();
            Assert.NotEqual(Guid.Empty, request.Id);
            Assert.True(request.Options != null && request.Options.Count == 0);
        }

        [Fact]
        public void Options()
        {
            var request = new TestRequest();
            request.Options.Add("opt1", 1);
            request.Options.Add("opt2", "two");
            request.Options.Add("opt3", 3.333);

            Assert.True(request.Options.Count == 3);
            Assert.Equal(1, request.Options["opt1"]);
            Assert.Equal("two", request.Options["opt2"]);
            Assert.Equal(3.333, request.Options["opt3"]);
        }

        private class TestRequest : Request
        {
        }
    }
}