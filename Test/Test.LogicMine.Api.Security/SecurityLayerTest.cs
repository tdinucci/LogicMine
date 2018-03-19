using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Filter;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.GetSingle;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;
using LogicMine.Api.Security;
using Test.LogicMine.Common.Layers;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Security
{
  public class SecurityLayerTest
  {
    [Fact]
    public async Task Get()
    {
      var mine = new FrogMine("allowed");
      var basket = new GetBasket<int, Frog>(8);
      await mine.SendAsync(basket);
      Assert.Equal(8, basket.AscentPayload.Id);

      mine = new FrogMine("Get");
      basket = new GetBasket<int, Frog>(8);
      await mine.SendAsync(basket);
      Assert.Equal(8, basket.AscentPayload.Id);

      mine = new FrogMine("not-allowed");
      basket = new GetBasket<int, Frog>(8);
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task GetSingle()
    {
      var mine = new FrogMine("allowed");
      var basket = new GetSingleBasket<Frog>();
      await mine.SendAsync(basket);
      Assert.Equal(465, basket.AscentPayload.Id);

      mine = new FrogMine("GetSingle");
      basket = new GetSingleBasket<Frog>();
      await mine.SendAsync(basket);
      Assert.Equal(465, basket.AscentPayload.Id);

      mine = new FrogMine("not-allowed");
      basket = new GetSingleBasket<Frog>();
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task GetCollection()
    {
      var mine = new FrogMine("allowed");
      var basket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(basket);
      Assert.Equal(3, basket.AscentPayload.Length);

      mine = new FrogMine("GetCollection");
      basket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(basket);
      Assert.Equal(3, basket.AscentPayload.Length);

      mine = new FrogMine("not-allowed");
      basket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task Post()
    {
      var mine = new FrogMine("allowed");
      var basket = new PostBasket<Frog, int>(new Frog());
      await mine.SendAsync(basket);
      Assert.True(basket.AscentPayload > 0);

      mine = new FrogMine("Post");
      basket = new PostBasket<Frog, int>(new Frog());
      await mine.SendAsync(basket);
      Assert.True(basket.AscentPayload > 0);

      mine = new FrogMine("not-allowed");
      basket = new PostBasket<Frog, int>(new Frog());
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task Put()
    {
      var mine = new FrogMine("allowed");
      var basket = new PutBasket<int, Frog, int>(new PutRequest<int, Frog>(2, new Frog()));
      await mine.SendAsync(basket);
      Assert.Equal(1, basket.AscentPayload);

      mine = new FrogMine("Put");
      basket = new PutBasket<int, Frog, int>(new PutRequest<int, Frog>(2, new Frog()));
      await mine.SendAsync(basket);
      Assert.Equal(1, basket.AscentPayload);

      mine = new FrogMine("not-allowed");
      basket = new PutBasket<int, Frog, int>(new PutRequest<int, Frog>(2, new Frog()));
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task Patch()
    {
      var mine = new FrogMine("allowed");
      var basket = new PatchBasket<int, Frog, int>(new PatchRequest<int, Frog>(new Delta<int, Frog>(5,
        new Dictionary<string, object> {{nameof(Frog.Name), "abc"}})));
      await mine.SendAsync(basket);
      Assert.Equal(1, basket.AscentPayload);

      mine = new FrogMine("Patch");
      basket = new PatchBasket<int, Frog, int>(new PatchRequest<int, Frog>(new Delta<int, Frog>(5,
        new Dictionary<string, object> {{nameof(Frog.Name), "abc"}})));
      await mine.SendAsync(basket);
      Assert.Equal(1, basket.AscentPayload);

      mine = new FrogMine("not-allowed");
      basket = new PatchBasket<int, Frog, int>(new PatchRequest<int, Frog>(new Delta<int, Frog>(5,
        new Dictionary<string, object> {{nameof(Frog.Name), "abc"}})));
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task Delete()
    {
      var mine = new FrogMine("allowed");
      var basket = new DeleteBasket<int, Frog, int>(6);
      await mine.SendAsync(basket);
      Assert.Equal(1, basket.AscentPayload);

      mine = new FrogMine("Delete");
      basket = new DeleteBasket<int, Frog, int>(6);
      await mine.SendAsync(basket);
      Assert.Equal(1, basket.AscentPayload);

      mine = new FrogMine("not-allowed");
      basket = new DeleteBasket<int, Frog, int>(6);
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    [Fact]
    public async Task DeleteCollection()
    {
      var mine = new FrogMine("allowed");
      var basket = new DeleteCollectionBasket<Frog, int>(new DeleteCollectionRequest<Frog>(new Filter<Frog>(new[]
      {
        new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "abc")
      })));

      await mine.SendAsync(basket);
      Assert.Equal(5, basket.AscentPayload);

      mine = new FrogMine("DeleteCollection");
      basket = new DeleteCollectionBasket<Frog, int>(new DeleteCollectionRequest<Frog>(new Filter<Frog>(new[]
      {
        new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "abc")
      })));
      await mine.SendAsync(basket);
      Assert.Equal(5, basket.AscentPayload);

      mine = new FrogMine("not-allowed");
      basket = new DeleteCollectionBasket<Frog, int>(new DeleteCollectionRequest<Frog>(new Filter<Frog>(new[]
      {
        new FilterTerm(nameof(Frog.Name), FilterOperators.StartsWith, "abc")
      })));
      await Assert.ThrowsAnyAsync<ShaftException>(() => mine.SendAsync(basket));
    }

    private class SimpleFrogSecurityLayer : SecurityLayer<Frog, string>
    {
      public SimpleFrogSecurityLayer(string user) : base(user)
      {
      }

      protected override bool IsOperationAllowed(string user, Operations operation)
      {
        var allowed = user == "allowed" || user == operation.ToString();
        return allowed;
      }
    }

    private class FrogMine : Mine<Frog>,
      IGetShaft<int, Frog>,
      IGetSingleShaft<Frog>,
      IGetCollectionShaft<Frog>,
      IPostShaft<Frog, int>,
      IPutShaft<int, Frog, int>,
      IPatchShaft<int, Frog, int>,
      IDeleteShaft<int, int>,
      IDeleteCollectionShaft<Frog,int>
    {
      private SimpleFrogSecurityLayer SecurityLayer { get; }
      private new SimpleFrogTerminalLayer TerminalLayer => (SimpleFrogTerminalLayer) base.TerminalLayer;

      public FrogMine(string userToken) : base(new SimpleFrogTerminalLayer())
      {
        SecurityLayer = new SimpleFrogSecurityLayer(userToken);
      }

      public Task<IGetBasket<int, Frog>> SendAsync(IGetBasket<int, Frog> basket)
      {
        return new Shaft<IGetBasket<int, Frog>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IGetSingleBasket<Frog>> SendAsync(IGetSingleBasket<Frog> basket)
      {
        return new Shaft<IGetSingleBasket<Frog>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IGetCollectionBasket<Frog>> SendAsync(IGetCollectionBasket<Frog> basket)
      {
        return new Shaft<IGetCollectionBasket<Frog>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IPostBasket<Frog, int>> SendAsync(IPostBasket<Frog, int> basket)
      {
        return new Shaft<IPostBasket<Frog, int>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IPutBasket<int, Frog, int>> SendAsync(IPutBasket<int, Frog, int> basket)
      {
        return new Shaft<IPutBasket<int, Frog, int>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IPatchBasket<int, Frog, int>> SendAsync(IPatchBasket<int, Frog, int> basket)
      {
        return new Shaft<IPatchBasket<int, Frog, int>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IDeleteBasket<int, int>> SendAsync(IDeleteBasket<int, int> basket)
      {
        return new Shaft<IDeleteBasket<int, int>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }

      public Task<IDeleteCollectionBasket<Frog, int>> SendAsync(IDeleteCollectionBasket<Frog, int> basket)
      {
        return new Shaft<IDeleteCollectionBasket<Frog, int>>(TerminalLayer)
          .Add(SecurityLayer)
          .SendAsync(basket);
      }
    }
  }
}