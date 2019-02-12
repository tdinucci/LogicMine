using System;
using LogicMine.DataObject.Filter;
using Microsoft.Data.Sqlite;

namespace LogicMine.DataObject.Ado.Sqlite
{
    /// <summary>
    /// Used to convert IFilters to IDbFilters suitable for use with SQLite
    /// </summary>
    public class SqliteFilterGenerator : DbFilterGenerator<SqliteParameter>
    {
        /// <summary>
        /// Construct a new SqliteFilterGenerator
        /// </summary>
        /// <param name="filter">The IFilter to convert</param>
        public SqliteFilterGenerator(IFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Construct a new SqliteFilterGenerator
        /// </summary>
        /// <param name="filter">The IFilter to convert</param>
        /// <param name="covertPropToFieldName">A function that can map IFilter term properties to database column names</param>
        public SqliteFilterGenerator(IFilter filter, Func<string, string> covertPropToFieldName) :
            base(filter, covertPropToFieldName)
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

            return new SqliteParameter(name, value);
        }

        /// <summary>
        /// Get a new DbFilter specifically for use with SQLite
        /// </summary>
        /// <param name="clause">The WHERE clause</param>
        /// <param name="parameters">The parameters which participate in the WHERE clause</param>
        /// <returns></returns>
        protected override IDbFilter<SqliteParameter> GetDbFilter(string clause, SqliteParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(clause))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clause));

            return new DbFilter<SqliteParameter>(clause, parameters);
        }
    }
}