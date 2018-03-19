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
  /// A type which contains metadata related to objects which are stored in a database.
  /// </summary>
  /// <typeparam name="T">The type described</typeparam>
  public abstract class DbObjectDescriptor<T> : IDbObjectDescriptor<T>
  {
    /// <summary>
    /// A collection of property names which should not be written to the database
    /// </summary>
    protected HashSet<string> ReadOnlyPropertyNames { get; }

    /// <summary>
    /// Construct a new DbObjectDescriptor
    /// </summary>
    protected DbObjectDescriptor()
    {
    }

    /// <summary>
    /// Construct a new DbObjectDescriptor
    /// </summary>
    /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
    protected DbObjectDescriptor(IEnumerable<string> readOnlyPropertyNames)
    {
      ReadOnlyPropertyNames = new HashSet<string>(readOnlyPropertyNames ?? new string[0]);
    }

    /// <inheritdoc />
    public virtual bool CanWrite(string propertyName)
    {
      if (ReadOnlyPropertyNames != null)
        return !ReadOnlyPropertyNames.Contains(propertyName);

      return true;
    }

    /// <inheritdoc />
    public virtual bool CanRead(string propertyName)
    {
      return true;
    }

    /// <inheritdoc />
    public virtual string GetMappedColumnName(string propertyName)
    {
      return propertyName;
    }

    /// <inheritdoc />
    public virtual object ProjectColumnValue(object columnValue, Type propertyType)
    {
      return columnValue;
    }

    /// <inheritdoc />
    public TProjected ProjectColumnValue<TProjected>(object columnValue)
    {
      return (TProjected) ProjectColumnValue(columnValue, typeof(TProjected));
    }

    /// <inheritdoc />
    public virtual object ProjectPropertyValue(object propertyValue, string propertyName)
    {
      return propertyValue;
    }
  }
}
