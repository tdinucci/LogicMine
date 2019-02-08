namespace LogicMine.DataObject.Filter
{
  /// <summary>
  /// A term within a filter, a term is a constraint placed on a single property
  /// </summary>
  public interface IFilterTerm
  {
    /// <summary>
    /// The name of the property the term applies to
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// The type of filtering to perform, e.g. equal, less-than, etc.
    /// </summary>
    FilterOperators Operator { get; }

    /// <summary>
    /// The property value to filter on
    /// </summary>
    object Value { get; }
  }
}