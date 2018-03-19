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
using System.Collections.Generic;

namespace LogicMine.Api.Filter
{
  /// <summary>
  /// A filter bound to type T.  If any terms do not match the constraints imposed by T (i.e. property name and type) then the filter 
  /// will be considered invalid
  /// </summary>
  /// <typeparam name="T">The type the filter is bound to</typeparam>
  public interface IFilter<T> : IFilter
  {
    /// <summary>
    /// The type which the filter is bound to
    /// </summary>
    Type FilteredType { get; }

    /// <summary>
    /// Convert the current filter to a filter on a different (but amost certainly similar in description) type
    /// </summary>
    /// <typeparam name="U">The type to bind the converted filter to</typeparam>
    /// <param name="propertyNameConverter">A delegate to convert property names from the current filter into property names for the new filter</param>
    /// <returns>A filter bound to U which is based on the current filter</returns>
    IFilter<U> Convert<U>(Func<string, string> propertyNameConverter);
  }

  /// <summary>
  /// A filter 
  /// </summary>
  public interface IFilter
  {
    /// <summary>
    /// A collection of terms within the current filter
    /// </summary>
    IEnumerable<IFilterTerm> Terms { get; }

    /// <summary>
    /// Adds a term to the current filter
    /// </summary>
    /// <param name="term"></param>
    void AddTerm(IFilterTerm term);
  }
}
