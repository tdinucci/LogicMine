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
using LogicMine.Api.Filter;

namespace LogicMine.Api.GetCollection
{
  /// <summary>
  ///  A request to perform a "Get Collection" operation on the set of T.
  /// </summary>
  /// <typeparam name="T">The type to get</typeparam>
  public interface IGetCollectionRequest<T>
  {
    /// <summary>
    /// True if there is no value for Filter or Max
    /// </summary>
    bool GetAll { get; }

    /// <summary>
    /// A filter that will be applied to the set of T
    /// </summary>
    IFilter<T> Filter { get; }

    /// <summary>
    /// The maximum number of results to return, null means all results
    /// </summary>
    int? Max { get; }

    /// <summary>
    /// The page within the results to return.  If Max is not specified then this value will be ignored
    /// </summary>
    int? Page { get; }
  }
}
