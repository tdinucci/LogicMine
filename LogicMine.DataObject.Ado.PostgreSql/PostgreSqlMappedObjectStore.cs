using System;
using LogicMine.DataObject.Filter;
using Npgsql;

namespace LogicMine.DataObject.Ado.PostgreSql
{
    /// <summary>
    /// An object store for T's which are mapped to a single PostgreSql table
    /// </summary>
    /// <typeparam name="T">The type which the terminals operate on</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class PostgreSqlMappedObjectStore<T, TId> : MappedAdoObjectStore<T, TId, NpgsqlParameter>
        where T : new()
    {
        /// <inheritdoc />
        protected override string SafeIdentifierFormat => "\"{0}\"";

        /// <summary>
        /// Construct a new PostgreSqlMappedObjectStore
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
        /// <param name="mapper">An object-relational mapper</param>
        /// <param name="transientErrorAwareExecutor">If provided all database operations will be executed within it.  The implementation will dictate things like the retry policy</param>
        public PostgreSqlMappedObjectStore(string connectionString, PostgreSqlMappedObjectDescriptor<T, TId> descriptor,
            IDbMapper<T> mapper = null, ITransientErrorAwareExecutor transientErrorAwareExecutor = null) :
            base(new PostgreSqlInterface(connectionString, transientErrorAwareExecutor), descriptor,
                mapper ?? new DbMapper<T>(descriptor))
        {
        }

        /// <summary>
        /// Get a new NpgsqlParameter based on the provided arguments
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <returns>A new NpgsqlParameter</returns>
        protected override NpgsqlParameter GetDbParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            return new NpgsqlParameter(name, value ?? DBNull.Value);
        }

        /// <inheritdoc />
        protected override IDbFilter<NpgsqlParameter> GetDbFilter(IFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return new PostgreSqlFilterGenerator(filter, (n) => MakeColumnNameSafe(Descriptor.GetMappedColumnName(n)))
                .Generate();
        }

        /// <inheritdoc />
        protected override string GetSelectLastIdentityQuery()
        {
            return $"returning {Descriptor.PrimaryKey}";
        }

        /// <inheritdoc />
        protected override IDbStatement<NpgsqlParameter> GetSelectDbStatement(IFilter<T> filter, int? max = null,
            int? page = null, string[] fields = null)
        {
            var maxRecords = max.GetValueOrDefault(0);
            if (maxRecords > 0)
            {
                if (filter != null)
                    return GetSelectDbStatement(filter, maxRecords, page.GetValueOrDefault(0), fields);

                return GetSelectDbStatement(maxRecords, page.GetValueOrDefault(0), fields);
            }

            return base.GetSelectDbStatement(filter, max, page);
        }

        private IDbStatement<NpgsqlParameter> GetSelectDbStatement(int max, int page, string[] fields)
        {
            var query =
                $"SELECT {GetSelectableColumns(fields)} FROM {Descriptor.Table} ORDER BY {Descriptor.PrimaryKey} " +
                $"LIMIT {max} OFFSET {max} * {page};";

            return new DbStatement<NpgsqlParameter>(query);
        }

        private IDbStatement<NpgsqlParameter> GetSelectDbStatement(IFilter<T> filter, int max, int page,
            string[] fields)
        {
            var sqlFilter = GetDbFilter(filter);

            var query = $"SELECT {GetSelectableColumns(fields)} FROM {Descriptor.Table} " +
                        $"{sqlFilter.WhereClause} ORDER BY {Descriptor.PrimaryKey} " +
                        $"LIMIT {max} OFFSET {max} * {page};";

            return new DbStatement<NpgsqlParameter>(query, sqlFilter.Parameters);
        }
    }
}