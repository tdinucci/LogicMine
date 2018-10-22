using Npgsql;

namespace LogicMine.Api.Data.Postgres
{
    /// <summary>
    /// A terminal layer for dealing with T's which are stored in a Postgres database
    /// </summary>
    /// <typeparam name="TId">The identity type on T</typeparam>
    /// <typeparam name="T">The type which the terminals operate on</typeparam>
    public abstract class PostgresLayer<TId, T> : DbLayer<TId, T, NpgsqlParameter>
        where T : new()
    {
        protected override string SafeIdentifierFormat => "\"{0}\"";

        /// <summary>
        /// Construct a new PostgresLayer
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
        /// <param name="mapper">An object-relational mapper</param>
        protected PostgresLayer(string connectionString, PostgresObjectDescriptor<T> descriptor, IDbMapper<T> mapper) :
            base(new PostgresInterface(connectionString), descriptor, mapper)
        {
        }
    }
}
