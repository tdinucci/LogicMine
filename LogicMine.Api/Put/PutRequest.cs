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

namespace LogicMine.Api.Put
{
  /// <summary>
  /// A request to "Put" an object
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type to put</typeparam>
  public class PutRequest<TId, T> : IPutRequest<TId, T>
  {
    /// <inheritdoc />
    public TId Identity { get; }

    /// <inheritdoc />
    public T Object { get; }

    /// <summary>
    /// Construct a new PutRequest
    /// </summary>
    /// <param name="identity">The identity of the object to put</param>
    /// <param name="obj">The object to put</param>
    public PutRequest(TId identity, T obj)
    {
      if (identity == null)
        throw new ArgumentNullException(nameof(identity));
      if (obj == null)
        throw new ArgumentNullException(nameof(obj));

      Identity = identity;
      Object = obj;
    }
  }
}
