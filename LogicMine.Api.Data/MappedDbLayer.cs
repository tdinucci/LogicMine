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
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;

namespace LogicMine.Api.Data
{
  /// <summary>
  /// A terminal layer for dealing with T's which are mapped to a single database table
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the terminals operate on</typeparam>
  /// <typeparam name="TDbParameter">The type of parameter used with the database</typeparam>
  public abstract class MappedDbLayer<TId, T, TDbParameter> : DbLayer<TId, T, TDbParameter>
    where T : new()
    where TDbParameter : IDbDataParameter
  {
    /// <summary>
    /// Contains metadata to enable mapping T's to database tables
    /// </summary>
    public new IMappedDbObjectDescriptor<T> Descriptor => (IMappedDbObjectDescriptor<T>) base.Descriptor;

    /// <summary>
    /// Construct a new MappedDbLayer
    /// </summary>
    /// <param name="dbInterface">An interface to an underlying database</param>
    /// <param name="descriptor">metadata to enable mapping T's to database tables</param>
    /// <param name="mapper">An object-relational mapper</param>
    protected MappedDbLayer(IDbInterface<TDbParameter> dbInterface, IMappedDbObjectDescriptor<T> descriptor,
      IDbMapper<T> mapper) : base(dbInterface, descriptor, mapper)
    {
    }

    /// <summary>
    /// Returns a database parameter for use with the utilised database
    /// </summary>
    /// <param name="name">The parameter name</param>
    /// <param name="value">The parameter value</param>
    /// <returns>A TDbParameter</returns>
    protected abstract TDbParameter GetDbParameter(string name, object value);

    /// <summary>
    /// Converts the provided IFilter into an IDbFilter
    /// </summary>
    /// <param name="filter">The filter to convert</param>
    /// <returns>An IDbFilter</returns>
    protected abstract IDbFilter<TDbParameter> GetDbFilter(IFilter filter);

    /// <summary>
    /// Returns the last identity value inserted into the database with the active connection
    /// </summary>
    /// <returns>The last database identity value within the current scope</returns>
    protected abstract string GetSelectLastIdentityQuery();

    /// <inheritdoc />
    protected override IDbStatement<TDbParameter> GetSelectDbStatement(TId identity)
    {
      var query =
        $"SELECT {GetSelectableColumns()} FROM {Descriptor.FullTableName} WHERE {Descriptor.PrimaryKey} = @Id";
      return new DbStatement<TDbParameter>(query, GetDbParameter("@Id", identity));
    }

    /// <summary>
    /// Get a statement to select all records of type T which match the request.
    /// 
    /// Different databases handle paged requests in different ways, so this implementation does not handle 
    /// paged requests and it is left up to sub classes to provide this.
    /// </summary>
    /// <param name="request">The "get collection" request</param>
    /// <returns>A statement to represent the "select" operation</returns>
    protected override IDbStatement<TDbParameter> GetSelectDbStatement(IGetCollectionRequest<T> request)
    {
      var query = $"SELECT {GetSelectableColumns()} FROM {Descriptor.FullTableName}";

      if (request.Filter != null)
      {
        var dbFilter = GetDbFilter(request.Filter);
        query += " " + dbFilter.WhereClause;

        return new DbStatement<TDbParameter>(query, dbFilter.Parameters);
      }

      return new DbStatement<TDbParameter>(query);
    }

    /// <inheritdoc />
    protected override IDbStatement<TDbParameter> GetInsertDbStatement(T obj)
    {
      var fieldNames = string.Empty;
      var parameterNames = string.Empty;
      var parameters = new List<TDbParameter>();

      var objectAccessor = ObjectAccessor.Create(obj);
      var typeAccessor = TypeAccessor.Create(typeof(T));
      var members = typeAccessor.GetMembers();

      foreach (var member in members)
      {
        var column = Descriptor.GetMappedColumnName(member.Name);
        if (string.IsNullOrWhiteSpace(column) || !Descriptor.CanWrite(member.Name))
          continue;

        var paramName = $"@{column}";
        var paramValue = Descriptor.ProjectPropertyValue(objectAccessor[member.Name], member.Name) ?? DBNull.Value;
        parameters.Add(GetDbParameter(paramName, paramValue));

        fieldNames += MakeColumnNameSafe(column) + ",";
        parameterNames += paramName + ",";
      }

      fieldNames = fieldNames.TrimEnd(',');
      parameterNames = parameterNames.TrimEnd(',');

      var sql = $"INSERT INTO {Descriptor.FullTableName} ({fieldNames}) VALUES ({parameterNames});\r\n" +
                $"{GetSelectLastIdentityQuery()}";

      return new DbStatement<TDbParameter>(sql, parameters.ToArray());
    }

    /// <inheritdoc />
    protected override IDbStatement<TDbParameter> GetUpdateDbStatement(IPatchRequest<TId, T> request)
    {
      var parameters = new List<TDbParameter>();

      var typeAccessor = TypeAccessor.Create(typeof(T));
      var members = typeAccessor.GetMembers();

      var assignments = string.Empty;
      foreach (var member in members.Where(m => request.Delta.ModifiedProperties.ContainsKey(m.Name)))
      {
        if (!Descriptor.CanWrite(member.Name))
          throw new InvalidOperationException($"Cannot write to '{member.Name}'");

        var column = Descriptor.GetMappedColumnName(member.Name);
        if (string.IsNullOrWhiteSpace(column))
          continue;

        var paramName = $"@{column}";
        assignments += $"{MakeColumnNameSafe(column)} = {paramName},";

        var paramValue = Descriptor.ProjectPropertyValue(request.Delta.ModifiedProperties[member.Name], member.Name) ??
                         DBNull.Value;

        parameters.Add(GetDbParameter(paramName, paramValue));
      }

      assignments = assignments.TrimEnd(',');
      var sql = $"UPDATE {Descriptor.FullTableName} SET {assignments} WHERE {Descriptor.PrimaryKey} = @Id";

      parameters.Add(GetDbParameter("@Id", request.Delta.Identity));

      return new DbStatement<TDbParameter>(sql, CommandType.Text, parameters.ToArray());
    }

    /// <inheritdoc />
    protected override IDbStatement<TDbParameter> GetDeleteDbStatement(TId identity)
    {
      var statement = $"DELETE FROM {Descriptor.FullTableName} WHERE {Descriptor.PrimaryKey} = @Id";
      return new DbStatement<TDbParameter>(statement, CommandType.Text, GetDbParameter("@Id", identity));
    }

    /// <inheritdoc />
    protected override IDbStatement<TDbParameter> GetDeleteDbStatement(IDeleteCollectionRequest<T> request)
    {
      var dbFilter = GetDbFilter(request.Filter);

      var statement = $"DELETE FROM {Descriptor.FullTableName} {dbFilter.WhereClause}";
      return new DbStatement<TDbParameter>(statement, dbFilter.Parameters);
    }

    /// <summary>
    /// Returns a string formatted for a typical "select" statement which includes the columns that are readable
    /// </summary>
    /// <returns>A comma seperated string containing the selectable column names</returns>
    protected virtual string GetSelectableColumns()
    {
      var props = typeof(T).GetProperties();
      var colNames = props
        .Where(prop => Descriptor.CanRead(prop.Name))
        .Select(prop => MakeColumnNameSafe(Descriptor.GetMappedColumnName(prop.Name)))
        .Where(col => !string.IsNullOrWhiteSpace(col))
        .ToArray();

      if (colNames.Length == 0)
        throw new InvalidOperationException($"No selectable columns for '{typeof(T)}'");

      return string.Join(",", colNames);
    }
  }
}