using System;
using System.Collections.Generic;
using LogicMine.Api.Patch;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Patch
{
  public class PatchBasketTest
  {
    [Fact]
    public void Construct()
    {
      var delta = new Delta<int, Frog>(752, new Dictionary<string, object>
      {
        {nameof(Frog.Name), "abc"},
        {nameof(Frog.DateOfBirth), DateTime.Today}
      });

      var basket = new PatchBasket<int, Frog, int>(new PatchRequest<int, Frog>(delta));
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Same(delta, basket.DescentPayload.Delta);
    }
  }
}
