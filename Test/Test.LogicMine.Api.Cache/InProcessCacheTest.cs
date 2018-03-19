using System;
using System.Threading;
using LogicMine.Api.Cache;
using Xunit;

namespace Test.LogicMine.Api.Cache
{
  public class InProcessCacheTest
  {
    [Fact]
    public void General()
    {
      var cache = new InProcessCache();

      // ensure no error
      cache.Remove(Guid.NewGuid().ToString());

      Assert.ThrowsAny<ArgumentException>(() => cache.Store(null, 1));

      var key = Guid.NewGuid().ToString();
      Assert.False(cache.TryGet(key, out var _));

      cache.Store(key, this);

      Assert.True(cache.TryGet(key, out var test));
      Assert.Same(this, test);

      cache.Remove(key);

      Assert.False(cache.TryGet(key, out var _));
    }

    [Fact]
    public void ManyItems()
    {
      var itemCount = 10000;
      var cacheString = GetCacheValueString();

      var cache = new InProcessCache();
      for (var i = 0; i < itemCount; i++)
      {
        var key = GetCacheKey(i);
        var value = $"{key}{cacheString}";

        cache.Store(key, value);
      }

      for (var i = 0; i < itemCount; i++)
      {
        var key = GetCacheKey(i);
        var expectedValue = $"{key}{cacheString}";

        Assert.True(cache.TryGet(key, out var value));
        Assert.Equal(expectedValue, value);
      }
    }

    [Fact]
    public void Expiring()
    {
      var cache = new InProcessCache(TimeSpan.FromSeconds(1));

      cache.Store("key", "value");
      Assert.True(cache.TryGet("key", out var _));

      Thread.Sleep(300);
      Assert.True(cache.TryGet("key", out var _));

      Thread.Sleep(700);
      Assert.False(cache.TryGet("key", out var _));
    }

    private string GetCacheKey(int i)
    {
      return $"{i:D4}";
    }

    private string GetCacheValueString()
    {
      var result = string.Empty;
      for (var i = 0; i < 512; i++)
        result += "a";

      return result;
    }
  }
}
