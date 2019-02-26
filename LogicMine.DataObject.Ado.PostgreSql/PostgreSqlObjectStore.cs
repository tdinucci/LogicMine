using Npgsql;

namespace LogicMine.DataObject.Ado.PostgreSql
{
    /// <summary>
    /// An object store for T's which are stored in a PostgreSql database
    /// </summary>
    /// <typeparam name="T">The type which the terminals operate on</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public abstract class PostgreSqlObjectStore<T, TId> : AdoObjectStore<T, TId, NpgsqlParameter>
        where T : new()
    {
        /// <summary>
        /// Construct a new PostgreSqlObjectStore
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
        /// <param name="mapper">An object-relational mapper</param>
        protected PostgreSqlObjectStore(string connectionString, PostgreSqlObjectDescriptor<T, TId> descriptor,
            IDbMapper<T> mapper) : base(new PostgreSqlInterface(connectionString), descriptor, mapper)
        {
        }
    }
}