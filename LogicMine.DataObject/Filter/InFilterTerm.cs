using System.Collections.Generic;

namespace LogicMine.DataObject.Filter
{
  /// <inheritdoc />
  /// <summary>
  /// A specialised filter term for inclusion filtering
  /// </summary>
  public class InFilterTerm : FilterTerm
  {
    /// <summary>
    /// The collection of property values which are included in the filter
    /// </summary>
    public new IEnumerable<object> Value => (IEnumerable<object>) base.Value;

    /// <summary>
    /// Construct a new InFilterTerm
    /// </summary>
    /// <param name="propertyName">The name of the property the term applies to</param>
    /// <param name="value">The collection of property values which are included in the filter</param>
    public InFilterTerm(string propertyName, IEnumerable<object> value) :
      base(propertyName, FilterOperators.In, value)
    {
    }
  }
}
