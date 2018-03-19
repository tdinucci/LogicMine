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
namespace LogicMine.Api.DeleteCollection
{
  /// <summary>
  /// A basket for "Delete Collection" operations
  /// </summary>
  /// <typeparam name="T">The type to delete</typeparam>
  /// <typeparam name="TResut">The type of result added to the basket</typeparam>
  public interface IDeleteCollectionBasket<T, TResut> : IDeleteCollectionBasket<T>,
    IBasket<IDeleteCollectionRequest<T>, TResut>
  {
  }

  /// <summary>
  /// Construct a new IDeleteCollectionBasket
  /// </summary>
  /// <typeparam name="T">The type to delete</typeparam>
  public interface IDeleteCollectionBasket<T> : IDeleteCollectionBasket, IBasket<IDeleteCollectionRequest<T>>
  {
  }

  /// <summary>
  /// Construct a new IDeleteCollectionBasket
  /// </summary>
  public interface IDeleteCollectionBasket : IBasket
  {
  }
}
