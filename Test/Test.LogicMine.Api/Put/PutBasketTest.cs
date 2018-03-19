using System;
using LogicMine.Api.Put;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Put
{
  public class PutBasketTest
  {
    [Fact]
    public void Construct()
    {
      var frog = new Frog { Id = 75, Name = "Fred", DateOfBirth = DateTime.Today };
      var request = new PutRequest<int, Frog>(frog.Id, frog);

      var basket = new PutBasket<int, Frog, int>(request);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Equal(75, basket.DescentPayload.Identity);
      Assert.Same(frog, basket.DescentPayload.Object);
    }
  }
}
