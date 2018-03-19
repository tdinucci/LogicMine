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
using System.Threading.Tasks;
using LogicMine.Api.Delete;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.GetSingle;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;

namespace LogicMine.Api.Cache
{
  /// <summary>
  /// <para>
  /// A layer of stations which provide cache management.
  /// </para>
  /// <para>
  /// N.B. This layer does not contain a DeleteCollection station and as such is not compatible with mines that 
  /// contain a DeleteCollection shaft.
  /// </para>
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type operated on</typeparam>
  public class CacheLayer<TId, T> : IStationLayer<T>,
    IGetStation<IGetBasket<TId, T>>,
    IGetSingleStation<IGetSingleBasket<T>>,
    IGetCollectionStation<IGetCollectionBasket<T>>,
    IPostStation<IPostBasket<T, TId>>,
    IPutStation<IPutBasket<TId, T>>,
    IPatchStation<IPatchBasket<TId, T>>,
    IDeleteStation<IDeleteBasket<TId>>
  {
    /// <summary>
    /// The underlying cache store
    /// </summary>
    protected IObjectCache<TId, T> Cache { get; }

    /// <summary>
    /// Construct a new CacheLayer
    /// </summary>
    /// <param name="cache">The cache to use</param>
    public CacheLayer(IObjectCache<TId, T> cache)
    {
      Cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// When processing a "Get" request the cache will be checked to see if the requested object is there. 
    /// If it is then the cached object and a <see cref="ReturnNote"/> will be added to the basket 
    /// and this will cause the shaft to pull the basket back up again, saving on the basket having to decend further 
    /// down the shaft.
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IGetBasket<TId, T> basket)
    {
      if (basket.DescentPayload != null)
      {
        if (Cache.TryGet(basket.DescentPayload, out var obj))
        {
          basket.AscentPayload = obj;
          basket.ReplaceNote(new ReturnNote());
        }
      }

      return Task.CompletedTask;
    }

    /// <summary>
    /// After a "Get" request has been processed and the basket with the requested object has ascended 
    /// to here the object will be placed in the cache.
    /// </summary>
    /// <param name="basket">The response</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IGetBasket<TId, T> basket)
    {
      if (basket.AscentPayload != null)
        Cache.Store(basket.DescentPayload, basket.AscentPayload);

      return Task.CompletedTask;
    }

    /// <summary>
    /// No action performed
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IGetSingleBasket<T> basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// No action performed
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IGetSingleBasket<T> basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// When processing a "Get Collection" request the cache will be checked to see if the requested objects are there. 
    /// If they are then the cached objects and a <see cref="ReturnNote"/> will be added to the basket 
    /// and this will cause the shaft to pull the basket back up again, saving on the basket having to decend further 
    /// down the shaft.
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IGetCollectionBasket<T> basket)
    {
      if (basket?.DescentPayload != null && basket.DescentPayload.GetAll)
      {
        if (Cache.TryGetCollection(out var objs))
        {
          basket.AscentPayload = objs;
          basket.ReplaceNote(new ReturnNote());
        }
      }

      return Task.CompletedTask;
    }

    /// <summary>
    /// After a "Get Collection" request has been processed and the basket with the requested objects has ascended 
    /// to here the objects will be placed in the cache.
    /// </summary>
    /// <param name="basket">The response</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IGetCollectionBasket<T> basket)
    {
      if (basket?.AscentPayload != null && basket.DescentPayload.GetAll)
        Cache.StoreCollection(basket.AscentPayload);

      return Task.CompletedTask;
    }

    /// <summary>
    /// No action is performed
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IPostBasket<T, TId> basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// After a "Post" request has been processed and the response basket has ascended to here 
    /// any cached collection which is now invalid (i.e. does not contain the object but now should) 
    /// is removed from the cache
    /// </summary>
    /// <param name="basket">The response</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IPostBasket<T, TId> basket)
    {
      // always remove items from cache on way back up, otherwise another thread 
      // may repopulate the cache before the decent is complete
      Cache.RemoveCollection();

      return Task.CompletedTask;
    }

    /// <summary>
    /// No action performed
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IPutBasket<TId, T> basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// After a "Put" request has been processed and the response basket has ascended to here 
    /// any cached object and collection which is now invalid is removed from the cache
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IPutBasket<TId, T> basket)
    {
      // always remove items from cache on way back up, otherwise another thread 
      // may repopulate the cache before the decent is complete
      if (basket?.DescentPayload != null)
      {
        Cache.RemoveCollection();
        Cache.Remove(basket.DescentPayload.Identity);
      }

      return Task.CompletedTask;
    }

    /// <summary>
    /// No action performed
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IPatchBasket<TId, T> basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// After a "Patch" request has been processed and the response basket has ascended to here 
    /// any cached object and collection which is now invalid is removed from the cache
    /// </summary>
    /// <param name="basket">The response</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IPatchBasket<TId, T> basket)
    {
      // always remove items from cache on way back up, otherwise another thread 
      // may repopulate the cache before the decent is complete
      if (basket?.DescentPayload?.Delta != null)
      {
        Cache.RemoveCollection();
        Cache.Remove(basket.DescentPayload.Delta.Identity);
      }

      return Task.CompletedTask;
    }

    /// <summary>
    /// No action performed
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IDeleteBasket<TId> basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// After a "Delete" request has been processed and the response basket has ascended to here 
    /// any cached object and collection which is now invalid is removed from the cache
    /// </summary>
    /// <param name="basket">The response</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IDeleteBasket<TId> basket)
    {
      // always remote items from cache on way back up, otherwise another thread 
      // may repopulate the cache before the decent is complete
      if (basket.DescentPayload != null)
      {
        Cache.RemoveCollection();
        Cache.Remove(basket.DescentPayload);
      }

      return Task.CompletedTask;
    }
  }
}
