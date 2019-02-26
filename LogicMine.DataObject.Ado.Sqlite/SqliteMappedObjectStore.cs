using System;
using LogicMine.DataObject.Filter;
using Microsoft.Data.Sqlite;

namespace LogicMine.DataObject.Ado.Sqlite
{
    /// <summary>
    /// An object store for T's which are mapped to a single SQLite table
    /// </summary>
    /// <typeparam name="T">The type which the terminals operate on</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class SqliteMappedObjectStore<T, TId> : MappedAdoObjectStore<T, TId, SqliteParameter>
        where T : new()
    {
        /// <summary>
        /// Construct a new SqliteMappedObjectStore
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
        /// <param name="mapper">An object-relational mapper</param>
        public SqliteMappedObjectStore(string connectionString, SqliteMappedObjectDescriptor<T, TId> descriptor,
            IDbMapper<T> mapper = null) :
            base(new SqliteInterface(connectionString), descriptor, mapper ?? new DbMapper<T>(descriptor))
        {
        }

        /// <summary>
        /// Get a new SqliteParameter based on the provided arguments
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <returns>A new SqliteParameter</returns>
        protected override SqliteParameter GetDbParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            return new SqliteParameter(name, value ?? DBNull.Value);
        }

        /// <inheritdoc />
        protected override IDbFilter<SqliteParameter> GetDbFilter(IFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return new SqliteFilterGenerator(filter, (n) => MakeColumnNameSafe(Descriptor.GetMappedColumnName(n)))
                .Generate();
        }

        /// <inheritdoc />
        protected override string GetSelectLastIdentityQuery()
        {
            return "SELECT last_insert_rowid();";
        }

        /// <inheritdoc />
        protected override IDbStatement<SqliteParameter> GetSelectDbStatement(IFilter<T> filter, int? max = null,
            int? page = null)
        {
            var maxRecords = max.GetValueOrDefault(0);
            if (maxRecords > 0)
            {
                if (filter != null)
                    return GetSelectDbStatement(filter, maxRecords, page.GetValueOrDefault(0));

                return GetSelectDbStatement(maxRecords, page.GetValueOrDefault(0));
            }

            return base.GetSelectDbStatement(filter, max, page);
        }

        private IDbStatement<SqliteParameter> GetSelectDbStatement(int max, int page)
        {
            var query =
                $"SELECT {GetSelectableColumns()} FROM {Descriptor.Table} WHERE {Descriptor.PrimaryKey} NOT IN (" +
                $"SELECT {Descriptor.PrimaryKey} FROM {Descriptor.Table} ORDER BY {Descriptor.PrimaryKey} LIMIT {max * page}) " +
                $"ORDER BY {Descriptor.PrimaryKey} LIMIT {max}";

            return new DbStatement<SqliteParameter>(query);
        }

        private IDbStatement<SqliteParameter> GetSelectDbStatement(IFilter<T> filter, int max, int page)
        {
            var sqlFilter = GetDbFilter(filter);

            var query =
                $"SELECT {GetSelectableColumns()} FROM {Descriptor.Table} {sqlFilter.WhereClause} AND {Descriptor.PrimaryKey} NOT IN (" +
                $"SELECT {Descriptor.PrimaryKey} FROM {Descriptor.Table} {sqlFilter.WhereClause} ORDER BY {Descriptor.PrimaryKey} LIMIT {max * page}) " +
                $"ORDER BY {Descriptor.PrimaryKey} LIMIT {max}";

            return new DbStatement<SqliteParameter>(query, sqlFilter.Parameters);
        }
    }
}