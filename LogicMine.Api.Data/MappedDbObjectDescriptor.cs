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

namespace LogicMine.Api.Data
{
  /// <summary>
  /// A type which contains metadata related to objects which are mapped to a database table.
  /// </summary>
  /// <typeparam name="T">The type described</typeparam>
  public abstract class MappedDbObjectDescriptor<T> : DbObjectDescriptor, IMappedDbObjectDescriptor<T>
  {
    /// <inheritdoc />
    public abstract string Schema { get; }

    /// <inheritdoc />
    public virtual string Table { get; }

    /// <inheritdoc />
    public virtual string PrimaryKey { get; }

    /// <inheritdoc />
    public string FullTableName => $"{Schema}.{Table}";

    /// <summary>
    /// Construct a new DbObjectDescriptor
    /// </summary>
    /// <param name="table">The table T is mapped to</param>
    /// <param name="primaryKey">The primary key on the table T is mapped to</param>
    protected MappedDbObjectDescriptor(string table, string primaryKey) : this(table, primaryKey, null)
    {
    }

    /// <summary>
    /// Construct a new DbObjectDescriptor
    /// </summary>
    /// <param name="table">The table T is mapped to</param>
    /// <param name="primaryKey">The primary key on the table T is mapped to</param>
    /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
    protected MappedDbObjectDescriptor(string table, string primaryKey, IEnumerable<string> readOnlyPropertyNames) :
      base(readOnlyPropertyNames)
    {
      if (string.IsNullOrWhiteSpace(table))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(table));
      if (string.IsNullOrWhiteSpace(primaryKey))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(primaryKey));

      Table = table;
      PrimaryKey = primaryKey;
    }
  }
}
