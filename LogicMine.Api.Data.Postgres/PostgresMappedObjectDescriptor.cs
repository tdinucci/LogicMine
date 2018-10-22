using System;
using System.Runtime.InteropServices.ComTypes;

namespace LogicMine.Api.Data.Postgres
{
  /// <summary>
  /// A type which contains metadata related to objects which are mapped to Postgres tables.
  /// </summary>
  /// <typeparam name="T">The type described</typeparam>
  public class PostgresMappedObjectDescriptor<T> : PostgresObjectDescriptor<T>, IMappedDbObjectDescriptor<T>
  {
    /// <summary>
    /// The schema of the table which is mapped to type T.  By default this is "dbo"
    /// </summary>
    public virtual string Schema => "public";

    /// <inheritdoc />
    public virtual string Table { get; }

    /// <inheritdoc />
    public virtual string PrimaryKey { get; }

    /// <inheritdoc />
    public string FullTableName => $"{Schema}.{Table}";

    /// <summary>
    /// Construct a new PostgresMappedObjectDescriptor
    /// </summary>
    /// <param name="table">The table T is mapped to</param>
    /// <param name="primaryKey">The primary key on the table T is mapped to</param>
    public PostgresMappedObjectDescriptor(string table, string primaryKey) : this(table, primaryKey, null)
    {
    }

    /// <summary>
    /// Construct a new PostgresMappedObjectDescriptor
    /// </summary>
    /// <param name="table">The table T is mapped to</param>
    /// <param name="primaryKey">The primary key on the table T is mapped to</param>
    /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
    public PostgresMappedObjectDescriptor(string table, string primaryKey, params string[] readOnlyPropertyNames) :
      base(readOnlyPropertyNames)
    {
      if (string.IsNullOrWhiteSpace(table))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(table));
      if (string.IsNullOrWhiteSpace(primaryKey))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(primaryKey));

      Table = table;
      PrimaryKey = primaryKey;
    }

    /// <summary>
    /// Converts conventional C# property names to conventional Postgres field names, e.g.
    /// Name maps to name and DateOfBirth maps to date_of_birth
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public override string GetMappedColumnName(string propertyName)
    {
      var result = char.ToLower(propertyName[0]).ToString();
      for (var i = 1; i < propertyName.Length; i++)
      {
        var ch = propertyName[i];
        if (char.IsUpper(ch))
        {
          result += '_';
          result += char.ToLower(ch);
        }
        else
          result += ch;
      }

      return result;
    }
  }
}
