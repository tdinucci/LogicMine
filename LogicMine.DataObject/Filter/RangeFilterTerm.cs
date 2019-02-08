using System;

namespace LogicMine.DataObject.Filter
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
