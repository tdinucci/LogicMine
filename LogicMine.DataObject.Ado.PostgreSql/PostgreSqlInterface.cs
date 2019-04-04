using System;
using System.Transactions;
using Npgsql;

namespace LogicMine.DataObject.Ado.PostgreSql
{
    /// <summary>
    /// An general interface to Postgres
    /// </summary>
    public class PostgreSqlInterface : DbInterface<NpgsqlCommand, NpgsqlParameter>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Construct a new PostgreSqlInterface
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="transientErrorAwareExecutor">If provided all database operations will be executed within it.  The implementation will dictate things like the retry policy</param>
        public PostgreSqlInterface(string connectionString,
            ITransientErrorAwareExecutor transientErrorAwareExecutor = null) : base(transientErrorAwareExecutor)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns a new NpgsqlCommand based on the provided statement
        /// </summary>
        /// <param name="statement">The statement to convert to an NpgsqlCommand</param>
        /// <returns>A new NpgsqlCommand</returns>
        protected override NpgsqlCommand GetDbCommand(IDbStatement<NpgsqlParameter> statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return new NpgsqlCommand(statement.Text, new NpgsqlConnection(_connectionString));
        }
    }
}