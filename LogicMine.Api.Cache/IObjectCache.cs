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
namespace LogicMine.Api.Cache
{
  /// <summary>
  /// A cache which handles a particular type of object
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The handled type</typeparam>
  public interface IObjectCache<TId, T>
  {
    /// <summary>
    /// Get a T from the cache if it exists
    /// </summary>
    /// <param name="identity">The objects identity</param>
    /// <param name="obj">The cached object</param>
    /// <returns>True if the object was found, otherwise false</returns>
    bool TryGet(TId identity, out T obj);

    /// <summary>
    /// Get a collection of T from the cache if it exists
    /// </summary>
    /// <param name="objs">The cached collection</param>
    /// <returns>True if the collection was found, otherwise false</returns>
    bool TryGetCollection(out T[] objs);

    /// <summary>
    /// Store a T in the cache.  If there is already a matching entry then overwrite it
    /// </summary>
    /// <param name="identity">The obejcts identity</param>
    /// <param name="obj">The object to store</param>
    void Store(TId identity, T obj);

    /// <summary>
    /// Store a collection of T in the cache.  If there is already a maching collection then overwrite it
    /// </summary>
    /// <param name="objs"></param>
    void StoreCollection(T[] objs);

    /// <summary>
    /// Remove a T from the cache if it exists
    /// </summary>
    /// <param name="identity">The objects identity</param>
    void Remove(TId identity);

    /// <summary>
    /// Remove a collection of T from the cache if it exits
    /// </summary>
    void RemoveCollection();
  }
}
