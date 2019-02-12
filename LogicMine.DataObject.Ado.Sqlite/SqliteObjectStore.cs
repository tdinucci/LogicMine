using Microsoft.Data.Sqlite;

namespace LogicMine.DataObject.Ado.Sqlite
{
    /// <summary>
    /// An object store for T's which are stored in an SQLite database
    /// </summary>
    /// <typeparam name="T">The type which the terminals operate on</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public abstract class SqliteObjectStore<T, TId> : AdoObjectStore<T, TId, SqliteParameter>
        where T : new()
    {
        /// <summary>
        /// Construct a new SqliteDbLayer
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
        /// <param name="mapper">An object-relational mapper</param>
        protected SqliteObjectStore(string connectionString, SqliteObjectDescriptor<T, TId> descriptor,
            IDbMapper<T> mapper) : base(new SqliteInterface(connectionString), descriptor, mapper)
        {
        }
    }
}