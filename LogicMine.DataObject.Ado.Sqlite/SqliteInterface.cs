using System;
using Microsoft.Data.Sqlite;

namespace LogicMine.DataObject.Ado.Sqlite
{
    /// <summary>
    /// An general interface to SQLite
    /// </summary>
    public class SqliteInterface : DbInterface<SqliteCommand, SqliteParameter>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Construct a new SqliteInterface
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        /// <param name="transientErrorAwareExecutor">If provided all database operations will be executed within it.  The implementation will dictate things like the retry policy</param>
        public SqliteInterface(string connectionString,
            ITransientErrorAwareExecutor transientErrorAwareExecutor = null) : base(transientErrorAwareExecutor)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns a new SqlCommand based on the provided statement
        /// </summary>
        /// <param name="statement">The statement to convert to an SqlCommand</param>
        /// <returns>A new SqlCommand</returns>
        protected override SqliteCommand GetDbCommand(IDbStatement<SqliteParameter> statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return new SqliteCommand(statement.Text, new SqliteConnection(_connectionString));
        }
    }
}