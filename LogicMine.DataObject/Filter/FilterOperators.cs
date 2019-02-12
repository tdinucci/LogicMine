namespace LogicMine.DataObject.Filter
{
  /// <summary>
  /// Filter operators
  /// </summary>
  public enum FilterOperators
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    In,
    Range,
    StartsWith,
    EndsWith,
    Contains
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }
}