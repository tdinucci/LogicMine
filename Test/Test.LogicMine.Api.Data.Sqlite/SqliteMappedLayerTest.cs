using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogicMine.Api.Data;
using LogicMine.Api.Data.Sqlite;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Filter;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using Test.LogicMine.Api.Data.Sqlite.Util;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Data.Sqlite
{
  public class SqliteMappedLayerTest : IDisposable
  {
    private static readonly string DbFilename = $"{Path.GetTempPath()}\\testm.db";
    private DbGenerator _dbGenerator;

    private FrogLayer GetLayer()
    {
      _dbGenerator = new DbGenerator(DbFilename);
      return new FrogLayer(_dbGenerator.CreateDb("FrogId"));
    }

    private void InsertFrogs(FrogLayer layer, int count)
    {
      var tasks = new Task[count];
      for (var i = 1; i <= count; i++)
      {
        var frog = new Frog {Id = i, Name = $"Frank{i}", DateOfBirth = DateTime.Today.AddDays(-i)};
        var basket = new PostBasket<Frog, int>(frog);
        tasks[i - 1] = layer.AddResultAsync(basket);
      }

      Task.WaitAll(tasks);
    }

    [Fact]
    public async Task Get()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 10);

      var getBasket = new GetBasket<int, Frog>(5);
      await layer.AddResultAsync(getBasket).ConfigureAwait(false);

      Assert.Equal(5, getBasket.AscentPayload.Id);
      Assert.Equal($"Frank{5}", getBasket.AscentPayload.Name);
      Assert.Equal(DateTime.Today.AddDays(-5), getBasket.AscentPayload.DateOfBirth);
    }

    [Fact]
    public async Task GetAll()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 100);

      var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await layer.AddResultAsync(getBasket).ConfigureAwait(false);

      Assert.Equal(100, getBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task GetFiltered()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 100);

      var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>(new Filter<Frog>(new[]
        {new FilterTerm(nameof(Frog.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))})));

      await layer.AddResultAsync(getBasket).ConfigureAwait(false);

      Assert.Equal(50, getBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task GetPaged()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 100);

      for (var i = 0; i < 16; i++)
      {
        var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>(6, i));
        await layer.AddResultAsync(getBasket).ConfigureAwait(false);
        Assert.Equal(6, getBasket.AscentPayload.Length);
      }

      var getFinalBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>(6, 16));
      await layer.AddResultAsync(getFinalBasket).ConfigureAwait(false);
      Assert.Equal(4, getFinalBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task GetFilteredPaged()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 100);

      for (var i = 0; i < 8; i++)
      {
        var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>(new Filter<Frog>(new[]
          {new FilterTerm(nameof(Frog.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))}), 6, i));

        await layer.AddResultAsync(getBasket).ConfigureAwait(false);
        Assert.Equal(6, getBasket.AscentPayload.Length);
      }

      var getFinalBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>(new Filter<Frog>(new[]
        {new FilterTerm(nameof(Frog.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))}), 6, 8));

      await layer.AddResultAsync(getFinalBasket).ConfigureAwait(false);
      Assert.Equal(2, getFinalBasket.AscentPayload.Length);
    }

    [Fact]
    public async Task Patch()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 10);

      var patchBasket = new PatchBasket<int, Frog, int>(
        new PatchRequest<int, Frog>(new Delta<int, Frog>(7,
          new Dictionary<string, object> {{nameof(Frog.Name), "Patched"}})));

      await layer.AddResultAsync(patchBasket).ConfigureAwait(false);
      Assert.Equal(1, patchBasket.AscentPayload);

      var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await layer.AddResultAsync(getBasket).ConfigureAwait(false);

      var seenPatched = false;
      Assert.Equal(10, getBasket.AscentPayload.Length);
      foreach (var frog in getBasket.AscentPayload)
      {
        if (frog.Id == 7)
        {
          Assert.Equal("Patched", frog.Name);
          seenPatched = true;
        }
        else
          Assert.Equal($"Frank{frog.Id}", frog.Name);
      }

      Assert.True(seenPatched);
    }

    [Fact]
    public async Task Delete()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 10);

      var deleteBasket = new DeleteBasket<int, Frog, int>(4);

      await layer.AddResultAsync(deleteBasket).ConfigureAwait(false);
      Assert.Equal(1, deleteBasket.AscentPayload);

      var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await layer.AddResultAsync(getBasket).ConfigureAwait(false);

      Assert.Equal(9, getBasket.AscentPayload.Length);
      foreach (var frog in getBasket.AscentPayload)
        Assert.NotEqual(4, frog.Id);
    }

    [Fact]
    public async Task DeleteCollection()
    {
      var layer = GetLayer();
      InsertFrogs(layer, 10);

      var deleteBasket = new DeleteCollectionBasket<Frog, int>(
        new DeleteCollectionRequest<Frog>(new Filter<Frog>(new[]
          {new InFilterTerm(nameof(Frog.Id), new object[] {2, 4, 6, 8})})));

      await layer.AddResultAsync(deleteBasket).ConfigureAwait(false);
      Assert.Equal(4, deleteBasket.AscentPayload);

      var getBasket = new GetCollectionBasket<Frog>(new GetCollectionRequest<Frog>());
      await layer.AddResultAsync(getBasket).ConfigureAwait(false);

      Assert.Equal(6, getBasket.AscentPayload.Length);
      foreach (var frog in getBasket.AscentPayload)
      {
        Assert.NotEqual(2, frog.Id);
        Assert.NotEqual(4, frog.Id);
        Assert.NotEqual(6, frog.Id);
        Assert.NotEqual(8, frog.Id);
      }
    }

    public void Dispose()
    {
      _dbGenerator?.Dispose();
    }

    private class FrogLayer : SqliteMappedLayer<int, Frog>
    {
      private static readonly FrogDescriptor ObjDescriptor = new FrogDescriptor();

      public FrogLayer(string connectionString) :
        base(connectionString, ObjDescriptor, new DbMapper<Frog>(ObjDescriptor))
      {
      }
    }

    private class FrogDescriptor : SqliteMappedObjectDescriptor<Frog>
    {
      public FrogDescriptor() : base("Frog", "FrogId", nameof(Frog.Id))
      {
      }

      public override string GetMappedColumnName(string propertyName)
      {
        if (propertyName == nameof(Frog.Id))
          return "FrogId";

        return base.GetMappedColumnName(propertyName);
      }
    }
  }
}
