/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using Microsoft.Extensions.Caching.Memory;

namespace LogicMine.Api.Cache
{
  /// <summary>
  /// A cache which operates within the processes own address space
  /// </summary>
  public class InProcessCache : ICache
  {
    private readonly IMemoryCache _cache;
    private readonly TimeSpan? _cacheEntryLifespan;

    /// <summary>
    /// Construct a new InProcessCache
    /// </summary>
    /// <param name="cacheEntryLifespan">The maximum age of a cache entry</param>
    public InProcessCache(TimeSpan? cacheEntryLifespan = null)
    {
      if (cacheEntryLifespan < TimeSpan.Zero)
        throw new ArgumentException($"{nameof(cacheEntryLifespan)} must be positive");

      _cacheEntryLifespan = cacheEntryLifespan;

      _cache = new MemoryCache(new MemoryCacheOptions());
    }

    /// <inheritdoc />
    public bool TryGet(string key, out object value)
    {
      return _cache.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public void Store(string key, object value)
    {
      if (_cacheEntryLifespan.HasValue)
        _cache.Set(key, value, _cacheEntryLifespan.Value);
      else
        _cache.Set(key, value);
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
      _cache.Remove(key);
    }
  }
}
