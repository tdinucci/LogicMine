using System;
using LogicMine.DataObject.CreateObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.CreateObject
{
    public class CreateObjectRequestTest
    {
        [Fact]
        public void Construct()
        {
            var frog = new Frog<int>();
            var request = new CreateObjectRequest<Frog<int>>(frog);

            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Equal(frog, request.Object);
        }

        [Fact]
        public void Construct_BadArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateObjectRequest<Frog<int>>(null));
        }
    }
}