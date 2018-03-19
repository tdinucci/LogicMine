﻿/*
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
namespace LogicMine.Api.Patch
{
  /// <summary>
  /// A basket for "Patch" operations
  /// </summary>
  /// <typeparam name="TId">The identity type of the object to patch</typeparam>
  /// <typeparam name="T">The type to patch</typeparam>
  /// <typeparam name="TResult">The type of result added to the basket</typeparam>
  public interface IPatchBasket<TId, T, TResult> : IPatchBasket<TId, T>, IBasket<IPatchRequest<TId, T>, TResult>
  {
  }

  /// <summary>
  /// A basket for "Patch" operations
  /// </summary>
  /// <typeparam name="TId">The identity type of the object to patch</typeparam>
  /// <typeparam name="T">The type to patch</typeparam>
  public interface IPatchBasket<TId, T> : IPatchBasket, IBasket<IPatchRequest<TId, T>>
  {
  }

  /// <summary>
  /// A basket for "Patch" operations
  /// </summary>
  public interface IPatchBasket : IBasket
  {
  }
}
