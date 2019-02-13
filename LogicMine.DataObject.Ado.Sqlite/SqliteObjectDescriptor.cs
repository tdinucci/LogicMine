using System;

namespace LogicMine.DataObject.Ado.Sqlite
{
  /// <summary>
  /// A type which contains metadata related to objects which are stored in an SQLite database.
  /// </summary>
  /// <typeparam name="T">The type described</typeparam>
  /// <typeparam name="TId">The identity type on T</typeparam>
  public class SqliteObjectDescriptor<T, TId> : DataObjectDescriptor<T, TId>
  {
    /// <summary>
    /// Construct a new SqliteObjectDescriptor
    /// </summary>
    public SqliteObjectDescriptor()
    {
    }

    /// <summary>
    /// Construct a new SqliteObjectDescriptor
    /// </summary>
    /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
    public SqliteObjectDescriptor(params string[] readOnlyPropertyNames) : base(readOnlyPropertyNames)
    {
    }

    /// <inheritdoc />
    public override object ProjectColumnValue(object columnValue, Type propertyType)
    {
      if (columnValue != null)
      {
        if ((propertyType == typeof(bool) || propertyType == typeof(bool?)) && !(columnValue is bool))
          return Convert.ToBoolean(columnValue);
        if ((propertyType == typeof(int) || propertyType == typeof(int?)) && !(columnValue is int))
          return Convert.ToInt32(columnValue);
        if ((propertyType == typeof(decimal) || propertyType == typeof(decimal?)) && !(columnValue is decimal))
          return Convert.ToDecimal(columnValue);
        if ((propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) && !(columnValue is DateTime))
          return Convert.ToDateTime(columnValue);
      }

      return base.ProjectColumnValue(columnValue, propertyType);
    }
  }
}