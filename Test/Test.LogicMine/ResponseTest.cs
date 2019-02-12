using System;
using LogicMine;
using Test.Common.LogicMine.Mine.GetTime;
using Xunit;

namespace Test.LogicMine
{
    public class ResponseTest
    {
        [Fact]
        public void Construct()
        {
            var request = new GetTimeRequest();
            var response = new Response(request);

            Assert.Equal(request.Id, response.RequestId);
            Assert.Null(response.Error);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public void Construct_Error()
        {
            var request = new GetTimeRequest();
            var error = Guid.NewGuid().ToString();
            var response = new Response(request, error);

            Assert.Equal(request.Id, response.RequestId);
            Assert.Equal(error, response.Error);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public void Construct_NoRequest()
        {
            // on the base Response this should be valid because we may be generating a response 
            // before the Request even exists, e.g. an error occurred parsing the Request
            var response = new Response(null);

            Assert.Equal(Guid.Empty, response.RequestId);
            Assert.Null(response.Error);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public void Construct_ErrorNoRequest()
        {
            // on the base Response this should be valid because we may be generating a response 
            // before the Request even exists, e.g. an error occurred parsing the Request
            var error = Guid.NewGuid().ToString();
            var response = new Response(null, error);

            Assert.Equal(Guid.Empty, response.RequestId);
            Assert.Equal(error, response.Error);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
        }
    }
}