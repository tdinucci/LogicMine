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

namespace LogicMine.Api.Get
{
  /// <summary>
  /// A basket for "Get" operations
  /// </summary>
  /// <typeparam name="TId">The type of identity on T</typeparam>
  /// <typeparam name="T">The type which the basket operates on</typeparam>
  public class GetBasket<TId, T> : Basket<TId, T>, IGetBasket<TId, T>
  {
    /// <inheritdoc />
    public override Type DataType => typeof(T);

    /// <summary>
    /// Construct a new GetBasket
    /// </summary>
    /// <param name="objectIdentity">The identity of the object to get</param>
    public GetBasket(TId objectIdentity) : base(objectIdentity)
    {
    }

    /// <summary>
    /// Construct a new GetBasket
    /// </summary>
    /// <param name="objectIdentity">The identity of the object to get</param>
    /// <param name="parent">The basket which is the parent of the current one</param>
    public GetBasket(TId objectIdentity, IBasket parent) : base(objectIdentity, parent)
    {
    }
  }
}
