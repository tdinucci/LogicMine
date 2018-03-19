using LogicMine.Api.Delete;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Delete
{
  public class DeleteBasketTest
  {
    [Fact]
    public void Construct()
    {
      var basket = new DeleteBasket<int, Frog, int>(85);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Equal(85, basket.DescentPayload);
    }
  }
}
