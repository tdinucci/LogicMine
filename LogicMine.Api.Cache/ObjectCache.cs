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

namespace LogicMine.Api.Cache
{
  /// <summary>
  /// A wrapper around an <see cref="ICache"/> which presents a type specific view of the underlying cache
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The handled type</typeparam>
  public class ObjectCache<TId, T> : IObjectCache<TId, T>
  {
    private readonly ICache _innerCache;

    /// <summary>
    /// Construct a new ObjectCache
    /// </summary>
    /// <param name="innerCache">The wrapped cache</param>
    public ObjectCache(ICache innerCache)
    {
      _innerCache = innerCache ?? throw new ArgumentNullException(nameof(innerCache));
    }

    /// <inheritdoc />
    public bool TryGet(TId identity, out T obj)
    {
      if (identity == null)
        throw new ArgumentNullException(nameof(identity));

      return TryGet(GetObjectKey(identity), out obj);
    }

    /// <inheritdoc />
    public bool TryGetCollection(out T[] objs)
    {
      return TryGet(GetCollectionKey(), out objs);
    }

    /// <inheritdoc />
    public void Store(TId identity, T obj)
    {
      if (identity == null)
        throw new ArgumentNullException(nameof(identity));

      _innerCache.Store(GetObjectKey(identity), obj);
    }

    /// <inheritdoc />
    public void StoreCollection(T[] objs)
    {
      _innerCache.Store(GetCollectionKey(), objs);
    }

    /// <inheritdoc />
    public void Remove(TId identity)
    {
      if (identity == null)
        throw new ArgumentNullException(nameof(identity));

      _innerCache.Remove(GetObjectKey(identity));
    }

    /// <inheritdoc />
    public void RemoveCollection()
    {
      _innerCache.Remove(GetCollectionKey());
    }

    private string GetObjectKey(TId identity)
    {
      return $"{typeof(T)}:{identity}";
    }

    private string GetCollectionKey()
    {
      return $"{typeof(T)}[]";
    }

    private bool TryGet<TCached>(string key, out TCached cached)
    {
      cached = default(TCached);
      if (_innerCache.TryGet(key, out var toCast))
      {
        if (toCast != null && !(toCast is TCached))
        {
          throw new InvalidOperationException(
            $"Expected object to be a '{typeof(TCached)}' but it was a '{toCast.GetType()}'");
        }

        cached = (TCached) toCast;
        return true;
      }

      return false;
    }
  }
}
