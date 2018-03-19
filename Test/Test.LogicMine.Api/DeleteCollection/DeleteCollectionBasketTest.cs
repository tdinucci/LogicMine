using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Filter;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.DeleteCollection
{
  public class DeleteCollectionBasketTest
  {
    [Fact]
    public void Construct()
    {
      var filter = new Filter<Frog>(new[] {new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "Fr")});
      var request = new DeleteCollectionRequest<Frog>(filter);
      Assert.Same(filter, request.Filter);

      var basket = new DeleteCollectionBasket<Frog, int>(request);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Same(request, basket.DescentPayload);
    }
  }
}
