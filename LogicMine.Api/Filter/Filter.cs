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
using System.Collections.ObjectModel;
using System.Linq;

namespace LogicMine.Api.Filter
{
  /// <summary>
  /// A filter which is used to identity objects within the set of all T's
  /// </summary>
  /// <typeparam name="T">The filtered type</typeparam>
  public class Filter<T> : Filter, IFilter<T>
  {
    /// <inheritdoc />
    public Type FilteredType => typeof(T);

    /// <summary>
    /// Construct a new Filter
    /// </summary>
    /// <param name="terms">The terms that are part of the filter</param>
    public Filter(IEnumerable<IFilterTerm> terms) : base(terms)
    {
    }

    /// <inheritdoc />
    public IFilter<U> Convert<U>(Func<string, string> propertyNameConverter)
    {
      var convertedTerms = new List<IFilterTerm>();
      foreach (var term in Terms)
      {
        IFilterTerm convertedTerm = null;
        var convertedField = propertyNameConverter(term.PropertyName);
        if (string.IsNullOrWhiteSpace(convertedField))
          throw new InvalidOperationException(
            $"No mapping from '{term.PropertyName}' on '{typeof(T)}' to '{typeof(U)}'");

        switch (term.Operator)
        {
          case FilterOperators.In:
            var inTerm = (InFilterTerm) term;
            convertedTerm = new InFilterTerm(convertedField, inTerm.Value);
            break;
          case FilterOperators.Range:
            var rangeTerm = (RangeFilterTerm) term;
            convertedTerm = new RangeFilterTerm(convertedField, rangeTerm.From, rangeTerm.To);
            break;
          default:
            convertedTerm = new FilterTerm(convertedField, term.Operator, term.Value);
            break;
        }

        convertedTerms.Add(convertedTerm);
      }

      return new Filter<U>(convertedTerms);
    }
  }

  /// <summary>
  /// A filter which is used to identity objects
  /// </summary>
  public class Filter : IFilter
  {
    private readonly List<IFilterTerm> _terms = new List<IFilterTerm>();

    /// <inheritdoc />
    public IEnumerable<IFilterTerm> Terms => new ReadOnlyCollection<IFilterTerm>(_terms);

    /// <summary>
    /// Construct a new Filter
    /// </summary>
    /// <param name="terms">The terms that are part of the filter</param>
    public Filter(IEnumerable<IFilterTerm> terms)
    {
      if (terms == null || !terms.Any())
        throw new ArgumentException($"'{nameof(terms)}' does not contain any items");

      _terms.AddRange(terms);
    }

    /// <inheritdoc />
    public void AddTerm(IFilterTerm term)
    {
      if (term == null)
        throw new ArgumentNullException(nameof(term));

      _terms.Add(term);
    }
  }
}
