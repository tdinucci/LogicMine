using System;
using LogicMine;
using Test.Common.LogicMine.Mine.GetTime;
using Xunit;

namespace Test.LogicMine
{
    public class ResponseFactoryTest
    {
        [Fact]
        public void Create()
        {
            var request = new GetTimeRequest();
            var response = ResponseFactory.Create<GetTimeResponse>(request);

            Assert.Equal(request.Id, response.RequestId);
            Assert.Null(response.Error);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public void CreateErrorResponse()
        {
            var request = new GetTimeRequest();
            var error = Guid.NewGuid().ToString();
            var response = ResponseFactory.Create<GetTimeResponse>(request, error);

            Assert.Equal(request.Id, response.RequestId);
            Assert.Equal(error, response.Error);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public void Create_InvalidResponse()
        {
            var request = new GetTimeRequest();
            var ex = Assert.Throws<InvalidOperationException>(() => ResponseFactory.Create<InvalidResponse>(request));
            Assert.Equal(
                $"There is no ctor on '{typeof(InvalidResponse)}' that accepts only an '{typeof(IRequest).Name}'",
                ex.Message);
        }

        private class InvalidResponse : Response
        {
            public InvalidResponse(IRequest request, string someArg) : base(request)
            {
            }
        }
    }
}