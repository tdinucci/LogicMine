using System;
using LogicMine.Api.Cache;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Cache
{
  public class ObjectCacheTest
  {
    [Fact]
    public void TryGet()
    {
      var procCache = new InProcessCache();
      var cache = new ObjectCache<int, Frog>(procCache);

      var frog = new Frog {Id = 6, Name = "Frank", DateOfBirth = DateTime.Today.AddDays(78)};
      cache.Store(frog.Id, frog);

      Assert.True(cache.TryGet(6, out var cacheFrog));
      Assert.Equal(frog, cacheFrog);

      Assert.False(cache.TryGet(9, out cacheFrog));
      Assert.Null(cacheFrog);
    }

    [Fact]
    public void TryGetCollection()
    {
      var procCache = new InProcessCache();
      var cache = new ObjectCache<int, Frog>(procCache);

      var frogs = new[]
      {

        new Frog {Id = 6, Name = "Frank", DateOfBirth = DateTime.Today.AddDays(1)},
        new Frog {Id = 7, Name = "Fred", DateOfBirth = DateTime.Today.AddDays(2)},
        new Frog {Id = 8, Name = "Fergil", DateOfBirth = DateTime.Today.AddDays(3)},
      };

      cache.StoreCollection(frogs);

      Assert.True(cache.TryGetCollection(out var cacheFrogs));
      Assert.Equal(3, cacheFrogs.Length);

      Assert.Contains(cacheFrogs, (f) => f.Id == 6 && f.Name == "Frank" && f.DateOfBirth == DateTime.Today.AddDays(1));
      Assert.Contains(cacheFrogs, (f) => f.Id == 7 && f.Name == "Fred" && f.DateOfBirth == DateTime.Today.AddDays(2));
      Assert.Contains(cacheFrogs, (f) => f.Id == 8 && f.Name == "Fergil" && f.DateOfBirth == DateTime.Today.AddDays(3));
    }

    [Fact]
    public void Remove()
    {
      var procCache = new InProcessCache();
      var cache = new ObjectCache<int, Frog>(procCache);

      var frog = new Frog {Id = 6, Name = "Frank", DateOfBirth = DateTime.Today.AddDays(78)};
      cache.Store(frog.Id, frog);

      Assert.True(cache.TryGet(6, out var cacheFrog));
      Assert.Equal(frog, cacheFrog);

      cache.Remove(6);
      Assert.False(cache.TryGet(6, out cacheFrog));
      Assert.Null(cacheFrog);
    }

    [Fact]
    public void RemoveCollection()
    {
      var procCache = new InProcessCache();
      var cache = new ObjectCache<int, Frog>(procCache);

      var frogs = new[]
      {

        new Frog {Id = 6, Name = "Frank", DateOfBirth = DateTime.Today.AddDays(1)},
        new Frog {Id = 7, Name = "Fred", DateOfBirth = DateTime.Today.AddDays(2)},
        new Frog {Id = 8, Name = "Fergil", DateOfBirth = DateTime.Today.AddDays(3)},
      };

      cache.StoreCollection(frogs);

      Assert.True(cache.TryGetCollection(out var cacheFrogs));
      Assert.Equal(3, cacheFrogs.Length);

      cache.RemoveCollection();
      Assert.False(cache.TryGetCollection(out cacheFrogs));
      Assert.Null(cacheFrogs);
    }
  }
}
