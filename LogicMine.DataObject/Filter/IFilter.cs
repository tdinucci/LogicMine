using System;
using System.Collections.Generic;

namespace LogicMine.DataObject.Filter
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
    /// Convert the current filter to a filter on a different (but almost certainly similar in description) type
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
