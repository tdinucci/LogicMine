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

using System;

namespace LogicMine.Api.Filter
{
  /// <inheritdoc />
  /// <summary>
  /// A specialised filter term for range filtering
  /// </summary>
  public class RangeFilterTerm : FilterTerm
  {
    /// <summary>
    /// Item1 = From
    /// Item2 = To
    /// </summary>
    protected new Tuple<IComparable, IComparable> Value => (Tuple<IComparable, IComparable>) base.Value;

    /// <summary>
    /// The value to filter from
    /// </summary>
    public IComparable From => Value?.Item1;

    /// <summary>
    /// The value to filter to
    /// </summary>
    public IComparable To => Value?.Item2;

    /// <summary>
    /// Construct a new RangeFilterTerm
    /// </summary>
    /// <param name="propertyName">The name of the property the term applies to</param>
    /// <param name="fromValue">The lowest value to be consider when filtering</param>
    /// <param name="toValue">The highest value to consider when filtering</param>
    public RangeFilterTerm(string propertyName, IComparable fromValue, IComparable toValue) :
      base(propertyName, FilterOperators.Range, new Tuple<IComparable, IComparable>(fromValue, toValue))
    {
      if (From == null || To == null)
        throw new ArgumentException($"The '{nameof(From)}' and '{nameof(To)}' values must both be set");

      if (From.GetType() != To.GetType())
        throw new ArgumentException($"The '{nameof(From)}' and '{nameof(To)}' values must be of the same type");
    }
  }
}
