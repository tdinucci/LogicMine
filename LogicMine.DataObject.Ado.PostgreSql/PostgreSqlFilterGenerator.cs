using System;
using LogicMine.DataObject.Filter;
using Npgsql;

namespace LogicMine.DataObject.Ado.PostgreSql
{
    /// <summary>
    /// Used to convert IFilters to IDbFilters suitable for use with Postgres
    /// </summary>
    public class PostgreSqlFilterGenerator : DbFilterGenerator<NpgsqlParameter>
    {
        /// <inheritdoc />
        protected override string StringConcatOperator { get; } = "||";

        /// <summary>
        /// Construct a new PostgreSqlFilterGenerator
        /// </summary>
        /// <param name="filter">The IFilter to convert</param>
        public PostgreSqlFilterGenerator(IFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Construct a new PostgreSqlFilterGenerator
        /// </summary>
        /// <param name="filter">The IFilter to convert</param>
        /// <param name="covertPropToFieldName">A function that can map IFilter term properties to database column names</param>
        public PostgreSqlFilterGenerator(IFilter filter, Func<string, string> covertPropToFieldName) :
            base(filter, covertPropToFieldName)
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

            return new NpgsqlParameter(name, value);
        }

        /// <summary>
        /// Get a new DbFilter specifically for use with Postgres
        /// </summary>
        /// <param name="clause">The WHERE clause</param>
        /// <param name="parameters">The parameters which participate in the WHERE clause</param>
        /// <returns></returns>
        protected override IDbFilter<NpgsqlParameter> GetDbFilter(string clause, NpgsqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(clause))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clause));

            return new DbFilter<NpgsqlParameter>(clause, parameters);
        }
    }
}