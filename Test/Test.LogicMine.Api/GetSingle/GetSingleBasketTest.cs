using LogicMine.Api.Filter;
using LogicMine.Api.GetSingle;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.GetSingle
{
  public class GetSingleBasketTest
  {
    [Fact]
    public void Construct()
    {
      var filter = new Filter<Frog>(new[] {new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "Fr")});
      var request = new GetSingleRequest<Frog>(filter);
      Assert.Same(filter, request.Filter);

      var basket = new GetSingleBasket<Frog>(request);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Same(request, basket.DescentPayload);

      basket = new GetSingleBasket<Frog>(new GetSingleRequest<Frog>());
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Null(basket.DescentPayload.Filter);
    }
  }
}
