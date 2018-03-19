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
using LogicMine.Api.Filter;

namespace LogicMine.Api.GetCollection
{
  /// <summary>
  ///  A request to perform a "Get Collection" operation on the set of T.
  /// </summary>
  /// <typeparam name="T">The type to get</typeparam>
  public class GetCollectionRequest<T> : IGetCollectionRequest<T>
  {
    /// <inheritdoc />
    public bool GetAll => Filter == null && Max == null;

    /// <inheritdoc />
    public IFilter<T> Filter { get; }

    /// <inheritdoc />
    public int? Max { get; }

    /// <inheritdoc />
    public int? Page { get; }

    /// <summary>
    /// Construct a new GetCollectionRequest
    /// </summary>
    public GetCollectionRequest()
    {
    }

    /// <summary>
    /// Construct a new GetCollectionRequest
    /// </summary>
    /// <param name="filter">The filter to apply to the set of T</param>
    public GetCollectionRequest(IFilter<T> filter) : this(filter, null, null)
    {
    }

    /// <summary>
    /// Construct a new GetCollectionRequest
    /// </summary>
    /// <param name="max">The maximum number of results to return</param>
    /// <param name="page">The page within the results</param>
    public GetCollectionRequest(int max, int page) : this(null, (int?) max, page)
    {
    }

    /// <summary>
    /// Construct a new GetCollectionRequest
    /// </summary>
    /// <param name="filter">The filter to apply to the set of T</param>
    /// <param name="max">The maximum number of results to return</param>
    /// <param name="page">The page within the results</param>
    public GetCollectionRequest(IFilter<T> filter, int max, int page) : this(filter, (int?) max, page)
    {
    }

    internal GetCollectionRequest(IFilter<T> filter, int? max, int? page)
    {
      if (max.GetValueOrDefault(1) <= 0)
        throw new ArgumentOutOfRangeException(nameof(max));
      if (page.GetValueOrDefault(1) < 0)
        throw new ArgumentOutOfRangeException(nameof(page));

      Filter = filter;
      Max = max;
      Page = page;
    }
  }
}