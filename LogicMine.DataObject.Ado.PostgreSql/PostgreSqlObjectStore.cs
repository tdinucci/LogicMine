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
        /// <param name="transientErrorAwareExecutor">If provided all database operations will be executed within it.  The implementation will dictate things like the retry policy</param>
        protected PostgreSqlObjectStore(string connectionString, PostgreSqlObjectDescriptor<T, TId> descriptor,
            IDbMapper<T> mapper, ITransientErrorAwareExecutor transientErrorAwareExecutor = null) :
            base(new PostgreSqlInterface(connectionString, transientErrorAwareExecutor), descriptor, mapper)
        {
        }
    }
}