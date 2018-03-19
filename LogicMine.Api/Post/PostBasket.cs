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

namespace LogicMine.Api.Post
{
  /// <summary>
  /// A basket for "Post" operations
  /// </summary>
  /// <typeparam name="T">The type being posted</typeparam>
  /// <typeparam name="TResult">The result of the post operation</typeparam>
  public class PostBasket<T, TResult> : Basket<T, TResult>, IPostBasket<T, TResult>
  {
    /// <inheritdoc />
    public override Type DataType => typeof(T);

    /// <summary>
    /// Construct a new PostBasket
    /// </summary>
    /// <param name="obj">The T to post</param>
    public PostBasket(T obj) : base(obj)
    {
    }

    /// <summary>
    /// Construct a new PostBasket
    /// </summary>
    /// <param name="obj">The T to post</param>
    /// <param name="parent">The basket which is the parent of the current one</param>
    public PostBasket(T obj, IBasket parent) : base(obj, parent)
    {
    }
  }
}
