using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FastMember;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// An object store for T's which are mapped to a single database table in a database that has an
    /// ADO.Net data provider
    /// </summary>
    /// <typeparam name="T">The type which the terminals operate on</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    /// <typeparam name="TDbParameter">The type of parameter used with the ADO.Net data provider</typeparam>
    public abstract class MappedAdoObjectStore<T, TId, TDbParameter> : AdoObjectStore<T, TId, TDbParameter>
        where T : new()
        where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// Contains metadata to enable mapping T's to database tables
        /// </summary>
        public new IMappedDataObjectDescriptor Descriptor => (IMappedDataObjectDescriptor) base.Descriptor;

        /// <summary>
        /// Construct a new MappedDbLayer
        /// </summary>
        /// <param name="dbInterface">An interface to an underlying database</param>
        /// <param name="descriptor">metadata to enable mapping T's to database tables</param>
        /// <param name="mapper">An object-relational mapper</param>
        protected MappedAdoObjectStore(IDbInterface<TDbParameter> dbInterface, IMappedDataObjectDescriptor descriptor,
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
        /// Different databases handle paging differently and so this implementation does not attempt this.
        /// Database specific implementations of this class should override this method and add support for paging.
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        /// <param name="max">The maximum number of records desired</param>
        /// <param name="page">The page within the results that is desired</param>
        /// <returns>A statement to represent the "select" operation</returns>
        protected override IDbStatement<TDbParameter> GetSelectDbStatement(IFilter<T> filter, int? max = null,
            int? page = null)
        {
            var query = $"SELECT {GetSelectableColumns()} FROM {Descriptor.FullTableName}";
            if (filter != null)
            {
                var dbFilter = GetDbFilter(filter);
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
                var paramValue = Descriptor.ProjectPropertyValue(objectAccessor[member.Name], member.Name) ??
                                 DBNull.Value;
                parameters.Add(GetDbParameter(paramName, paramValue));

                fieldNames += MakeColumnNameSafe(column) + ",";
                parameterNames += paramName + ",";
            }

            fieldNames = fieldNames.TrimEnd(',');
            parameterNames = parameterNames.TrimEnd(',');

            var sql =
                $"INSERT INTO {Descriptor.FullTableName} ({fieldNames}) VALUES ({parameterNames}) {GetSelectLastIdentityQuery()}";

            return new DbStatement<TDbParameter>(sql, parameters.ToArray());
        }

        /// <inheritdoc />
        protected override IDbStatement<TDbParameter> GetInsertDbStatement(IEnumerable<T> objs)
        {
            var objArray = objs?.ToArray();
            if (objArray == null || !objArray.Any())
                throw new ArgumentException($"Expected '{nameof(objs)}' to contain some objects");

            var parameters = new List<TDbParameter>();

            var typeAccessor = TypeAccessor.Create(typeof(T));
            var members = typeAccessor.GetMembers();

            // get just the fields names
            var fieldBuilder = new StringBuilder();
            foreach (var member in members)
            {
                var column = Descriptor.GetMappedColumnName(member.Name);
                if (string.IsNullOrWhiteSpace(column) || !Descriptor.CanWrite(member.Name))
                    continue;

                if (fieldBuilder.Length > 0)
                    fieldBuilder.Append(",");

                fieldBuilder.Append(MakeColumnNameSafe(column));
            }

            // now get the parameters for each object
            var paramBuilder = new StringBuilder();
            for (var i = 0; i < objArray.Length; i++)
            {
                var obj = objArray[i];
                var objectAccessor = ObjectAccessor.Create(obj);

                paramBuilder.Append("(");
                var isExistingParam = false;
                for (var j = 0; j < members.Count; j++)
                {
                    var member = members[j];
                    var column = Descriptor.GetMappedColumnName(member.Name);
                    if (string.IsNullOrWhiteSpace(column) || !Descriptor.CanWrite(member.Name))
                        continue;

                    var paramName = $"@{column}{i}";
                    var paramValue = Descriptor.ProjectPropertyValue(objectAccessor[member.Name], member.Name) ??
                                     DBNull.Value;
                    parameters.Add(GetDbParameter(paramName, paramValue));

                    if (isExistingParam)
                        paramBuilder.Append(",");

                    paramBuilder.Append(paramName);
                    isExistingParam = true;
                }

                paramBuilder.Append(")");
                if (i < objArray.Length - 1)
                    paramBuilder.AppendLine(",");
            }

            var sql =
                $"INSERT INTO {Descriptor.FullTableName} ({fieldBuilder}) VALUES {paramBuilder} {GetSelectLastIdentityQuery()}";

            return new DbStatement<TDbParameter>(sql, parameters.ToArray());
        }

        /// <inheritdoc />
        protected override IDbStatement<TDbParameter> GetUpdateDbStatement(TId identity,
            IDictionary<string, object> modifiedProperties)
        {
            modifiedProperties = modifiedProperties.ToDictionary(p => p.Key.ToLower(), p => p.Value);
            var parameters = new List<TDbParameter>();

            var typeAccessor = TypeAccessor.Create(typeof(T));
            var members = typeAccessor.GetMembers();

            var assignments = string.Empty;
            foreach (var member in members.Where(m => modifiedProperties.ContainsKey(m.Name.ToLower())))
            {
                if (!Descriptor.CanWrite(member.Name))
                    throw new InvalidOperationException($"Cannot write to '{member.Name}'");

                var column = Descriptor.GetMappedColumnName(member.Name);
                if (string.IsNullOrWhiteSpace(column))
                    continue;

                var paramName = $"@{column}";
                assignments += $"{MakeColumnNameSafe(column)} = {paramName},";

                var paramValue =
                    Descriptor.ProjectPropertyValue(modifiedProperties[member.Name.ToLower()], member.Name) ??
                    DBNull.Value;

                parameters.Add(GetDbParameter(paramName, paramValue));
            }

            assignments = assignments.TrimEnd(',');
            if (string.IsNullOrWhiteSpace(assignments))
                throw new InvalidOperationException("Failed to determine assignments for update statement");

            var sql = $"UPDATE {Descriptor.FullTableName} SET {assignments} WHERE {Descriptor.PrimaryKey} = @Id";

            parameters.Add(GetDbParameter("@Id", identity));

            return new DbStatement<TDbParameter>(sql, CommandType.Text, parameters.ToArray());
        }

        /// <inheritdoc />
        protected override IDbStatement<TDbParameter> GetDeleteDbStatement(TId identity)
        {
            var statement = $"DELETE FROM {Descriptor.FullTableName} WHERE {Descriptor.PrimaryKey} = @Id";
            return new DbStatement<TDbParameter>(statement, CommandType.Text, GetDbParameter("@Id", identity));
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