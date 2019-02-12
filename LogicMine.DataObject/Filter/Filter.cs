using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LogicMine.DataObject.Filter
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
