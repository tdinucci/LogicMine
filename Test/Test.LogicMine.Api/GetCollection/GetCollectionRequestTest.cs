using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.GetCollection
{
  public class GetCollectionRequestTest
  {
    [Fact]
    public void Construct()
    {
      var filter = new Filter<Frog>(new[] {new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "Fr")});

      var request = new GetCollectionRequest<Frog>();
      Assert.True(request.GetAll);
      Assert.Null(request.Max);
      Assert.Null(request.Page);
      Assert.Null(request.Filter);

      request = new GetCollectionRequest<Frog>(filter);
      Assert.False(request.GetAll);
      Assert.Null(request.Max);
      Assert.Null(request.Page);
      Assert.Same(filter, request.Filter);
      
      request = new GetCollectionRequest<Frog>(5, 6);
      Assert.False(request.GetAll);
      Assert.Equal(5, request.Max);
      Assert.Equal(6, request.Page);
      Assert.Null(request.Filter);

      request = new GetCollectionRequest<Frog>(filter, 5, 6);
      Assert.False(request.GetAll);
      Assert.Equal(5, request.Max);
      Assert.Equal(6, request.Page);
      Assert.Same(filter, request.Filter);
    }
  }
}
