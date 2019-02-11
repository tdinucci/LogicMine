using System;
using LogicMine;
using Xunit;

namespace Test.LogicMine
{
    public class ShaftExceptionTest
    {
        [Fact]
        public void Construct()
        {
            var message = Guid.NewGuid().ToString();
            var ex = new ShaftException(message);

            Assert.Equal(message, ex.Message);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void ConstructWithInnerException()
        {
            var message = Guid.NewGuid().ToString();
            var innerMessage = Guid.NewGuid().ToString();
            var innerEx = new InvalidOperationException(innerMessage);

            var ex = new ShaftException(message, innerEx);

            Assert.Equal(message, ex.Message);
            Assert.Equal(innerEx, ex.InnerException);
            Assert.Equal(innerMessage, ex.InnerException.Message);
        }
    }
}