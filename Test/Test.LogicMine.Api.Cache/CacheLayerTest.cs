using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Delete;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;
using Test.LogicMine.Common.Layers;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Cache
{
  public class CacheLayerTest
  {
    [Fact]
    public async Task Get()
    {
      var mine = new FrogMine();

      // uncached
      var basket = new GetBasket<int, Frog>(3);
      await mine.SendAsync(basket).ConfigureAwait(false);
      Assert.True(basket.Note is OkNote);
      Assert.Equal(3, basket.AscentPayload.Id);
      Assert.Equal("Kermit", basket.AscentPayload.Name);
      Assert.Equal(3, basket.Visits.Count);

      // cached
      basket = new GetBasket<int, Frog>(3);
      await mine.SendAsync(basket).ConfigureAwait(false);
      Assert.True(basket.Note is ReturnNote);
      Assert.Equal(3, basket.AscentPayload.Id);
      Assert.Equal("Kermit", basket.AscentPayload.Name);
      Assert.Equal(1, basket.Visits.Count);

      // uncached
      basket = new GetBasket<int, Frog>(4);
      await mine.SendAsync(basket).ConfigureAwait(false);
      Assert.True(basket.Note is OkNote);
      Assert.Equal(4, basket.AscentPayload.Id);
      Assert.Equal("Kermit", basket.AscentPayload.Name);
      Assert.Equal(3, basket.Visits.Count);
    }

    [Fact]
    public async Task GetCollection()
    {
      var mine = new FrogMine();

      // uncached
      var basket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(basket).ConfigureAwait(false);
      Assert.True(basket.Note is OkNote);
      Assert.Equal(3, basket.Visits.Count);
      Assert.Equal(3, basket.AscentPayload.Length);

      // cached
      basket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(basket).ConfigureAwait(false);
      Assert.True(basket.Note is ReturnNote);
      Assert.Equal(1, basket.Visits.Count);
      Assert.Equal(3, basket.AscentPayload.Length);
    }

    [Fact]
    public async Task Post()
    {
      var mine = new FrogMine();

      // uncached item
      var getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(3, getBasket.Visits.Count);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // cached item
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(1, getBasket.Visits.Count);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // uncached collection
      var getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.Visits.Count);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      // cached collection
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is ReturnNote);
      Assert.Equal(1, getCollectionBasket.Visits.Count);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new PostBasket<Frog, int>(new Frog {Id = 852})).ConfigureAwait(false);

      // get cache should still exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // but get collection shouldn't
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task Put()
    {
      var mine = new FrogMine();

      // uncached item
      var getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // cached item
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // uncached collection
      var getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      // cached collection
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is ReturnNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new PutBasket<int, Frog, int>(new PutRequest<int, Frog>(8, new Frog())))
        .ConfigureAwait(false);

      // get cache should still exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // but get collection shouldn't
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new PutBasket<int, Frog, int>(new PutRequest<int, Frog>(33, new Frog())))
        .ConfigureAwait(false);

      // get cache should now be cleared exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // and so should the get collection's
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task Patch()
    {
      var mine = new FrogMine();

      // uncached item
      var getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // cached item
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // uncached collection
      var getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      // cached collection
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is ReturnNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new PatchBasket<int, Frog, int>(
          new PatchRequest<int, Frog>(
            new Delta<int, Frog>(8, new Dictionary<string, object> {{nameof(Frog.Name), "a"}}))))
        .ConfigureAwait(false);

      // get cache should still exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // but get collection shouldn't
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new PatchBasket<int, Frog, int>(
          new PatchRequest<int, Frog>(
            new Delta<int, Frog>(33, new Dictionary<string, object> {{nameof(Frog.Name), "a"}}))))
        .ConfigureAwait(false);

      // get cache should now be cleared exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // and so should the get collection's
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task Delete()
    {
      var mine = new FrogMine();

      // uncached item
      var getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // cached item
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // uncached collection
      var getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      // cached collection
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is ReturnNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new DeleteBasket<int, Frog, int>(8)).ConfigureAwait(false);

      // get cache should still exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is ReturnNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // but get collection shouldn't
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);

      await mine.SendAsync(new DeleteBasket<int, Frog, int>(33)).ConfigureAwait(false);

      // get cache should now be cleared exist
      getBasket = new GetBasket<int, Frog>(33);
      await mine.SendAsync(getBasket).ConfigureAwait(false);
      Assert.True(getBasket.Note is OkNote);
      Assert.Equal(33, getBasket.AscentPayload.Id);

      // and so should the get collection's
      getCollectionBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await mine.SendAsync(getCollectionBasket).ConfigureAwait(false);
      Assert.True(getCollectionBasket.Note is OkNote);
      Assert.Equal(3, getCollectionBasket.AscentPayload.Length);
    }

    private class FrogMine : Mine<Frog>,
      IGetShaft<int, Frog>,
      IGetCollectionShaft<Frog>,
      IPostShaft<Frog, int>,
      IPutShaft<int, Frog, int>,
      IPatchShaft<int, Frog, int>,
      IDeleteShaft<int, int>
    {
      private CacheLayer<int, Frog> CacheLayer { get; }
      private new SimpleFrogTerminalLayer TerminalLayer => (SimpleFrogTerminalLayer) base.TerminalLayer;

      public FrogMine() : base(new SimpleFrogTerminalLayer())
      {
        CacheLayer = new CacheLayer<int, Frog>(new ObjectCache<int, Frog>(new InProcessCache()));
      }

      public Task<IGetBasket<int, Frog>> SendAsync(IGetBasket<int, Frog> basket)
      {
        return new Shaft<IGetBasket<int, Frog>>(TerminalLayer)
          .Add(CacheLayer)
          .SendAsync(basket);
      }

      public Task<IGetCollectionBasket<Frog>> SendAsync(IGetCollectionBasket<Frog> basket)
      {
        return new Shaft<IGetCollectionBasket<Frog>>(TerminalLayer)
          .Add(CacheLayer)
          .SendAsync(basket);
      }

      public Task<IPostBasket<Frog, int>> SendAsync(IPostBasket<Frog, int> basket)
      {
        return new Shaft<IPostBasket<Frog, int>>(TerminalLayer)
          .Add(CacheLayer)
          .SendAsync(basket);
      }

      public Task<IPutBasket<int, Frog, int>> SendAsync(IPutBasket<int, Frog, int> basket)
      {
        return new Shaft<IPutBasket<int, Frog, int>>(TerminalLayer)
          .Add(CacheLayer)
          .SendAsync(basket);
      }

      public Task<IPatchBasket<int, Frog, int>> SendAsync(IPatchBasket<int, Frog, int> basket)
      {
        return new Shaft<IPatchBasket<int, Frog, int>>(TerminalLayer)
          .Add(CacheLayer)
          .SendAsync(basket);
      }

      public Task<IDeleteBasket<int, int>> SendAsync(IDeleteBasket<int, int> basket)
      {
        return new Shaft<IDeleteBasket<int, int>>(TerminalLayer)
          .Add(CacheLayer)
          .SendAsync(basket);
      }
    }
  }
}
