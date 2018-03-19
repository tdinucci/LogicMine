using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.GetCollection
{
  public class GetCollectionBasketTest
  {
    [Fact]
    public void Construct()
    {
      var filter = new Filter<Frog>(new[] {new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "Fr")});
      var request = new GetCollectionRequest<Frog>(filter);
      
      var basket = new GetCollectionBasket<Frog>(request);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Same(request, basket.DescentPayload);
    }
  }
}
