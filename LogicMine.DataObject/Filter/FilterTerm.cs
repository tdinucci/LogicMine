using System;

namespace LogicMine.DataObject.Filter
{
  /// <summary>
  /// A term within a filter, a term is a constraint placed on a single property
  /// </summary>
  public class FilterTerm : IFilterTerm
  {
    /// <inheritdoc />
    public string PropertyName { get; }

    /// <inheritdoc />
    public FilterOperators Operator { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Construct a new FilterTerm
    /// </summary>
    /// <param name="propertyName">The name of the property the term applies to</param>
    /// <param name="op">The type of filtering to perform</param>
    /// <param name="value">The value to filter on</param>
    public FilterTerm(string propertyName, FilterOperators op, object value)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));

      PropertyName = propertyName;
      Operator = op;
      Value = value;
    }
  }
}
