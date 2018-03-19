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
  /// A cache
  /// </summary>
  public interface ICache
  {
    /// <summary>
    /// Get an object from the cache if it exists
    /// </summary>
    /// <param name="key">The objects key</param>
    /// <param name="value">The cached object</param>
    /// <returns>True if the key was found, otherwise false</returns>
    bool TryGet(string key, out object value);

    /// <summary>
    /// Store an object in the cache and associate it with a key.  If there is already a matching entry 
    /// then it's overwritten
    /// </summary>
    /// <param name="key">The key for the entry</param>
    /// <param name="value">The object to store</param>
    void Store(string key, object value);

    /// <summary>
    /// Remove an entry from the cache if it exists
    /// </summary>
    /// <param name="key"></param>
    void Remove(string key);
  }
}
