using System;
using Npgsql;

namespace LogicMine.Api.Data.Postgres
{
    /// <summary>
    /// An general interface to Postgres
    /// </summary>
    public class PostgresInterface : DbInterface<NpgsqlCommand, NpgsqlParameter>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Construct a new PostgresInterface
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        public PostgresInterface(string connectionString)
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
