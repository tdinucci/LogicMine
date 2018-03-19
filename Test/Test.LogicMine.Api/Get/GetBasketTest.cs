using LogicMine.Api.Get;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Get
{
  public class GetBasketTest
  {
    [Fact]
    public void Construct()
    {
      var basket = new GetBasket<int, Frog>(85);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Equal(85, basket.DescentPayload);
    }
  }
}
