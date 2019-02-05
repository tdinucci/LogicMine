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
using System.Data;
using System.Linq;
using FastMember;

namespace LogicMine.Api.Data
{
  /// <summary>
  /// A basic object-relational mapper
  /// </summary>
  /// <typeparam name="T">The type which the mapper operates on</typeparam>
  public class DbMapper<T> : IDbMapper<T> where T : new()
  {
    /// <summary>
    /// Contains metadata to enable mapping T's to database tables
    /// </summary>
    protected IDbObjectDescriptor Descriptor { get; }

    /// <summary>
    /// Construct a new DbMapper
    /// </summary>
    /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
    public DbMapper(IDbObjectDescriptor descriptor)
    {
      Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
    }

    /// <inheritdoc />
    public virtual T MapObject(IDataRecord record)
    {
      var result = new T();
      var accessor = TypeAccessor.Create(typeof(T));

      foreach (var member in accessor.GetMembers().Where(m => m.CanWrite && Descriptor.CanRead(m.Name)))
      {
        try
        {
          var columnName = Descriptor.GetMappedColumnName(member.Name);
          if (!string.IsNullOrWhiteSpace(columnName))
          {
            // TODO: Use a reference to an ordinals dictionary so that if this 
            // method is called in a loop it doesn't have to repeatedly work this out.
            var ordinal = record.GetOrdinal(columnName);
            if (ordinal >= 0)
            {
              var value = record[ordinal];
              value = value == DBNull.Value ? null : Descriptor.ProjectColumnValue(value, member.Type);

              accessor[result, member.Name] = value;
            }
          }
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Failed to map property '{member.Name}'", ex);
        }
      }

      return result;
    }

    /// <inheritdoc />
    public virtual T[] MapObjects(IDataReader reader)
    {
      var result = new List<T>();
      while (reader.Read())
        result.Add(MapObject(reader));

      return result.ToArray();
    }
  }
}
