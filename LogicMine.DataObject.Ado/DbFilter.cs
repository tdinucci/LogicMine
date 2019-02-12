using System;
using System.Data;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// Represents a database query filter
    /// </summary>
    /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
    public class DbFilter<TDbParameter> : IDbFilter<TDbParameter> where TDbParameter : IDbDataParameter
    {
        /// <inheritdoc />
        public string WhereClause { get; }

        /// <inheritdoc />
        public TDbParameter[] Parameters { get; }

        /// <summary>
        /// Construct a new DbFilter
        /// </summary>
        /// <param name="whereClause">The WHERE clause</param>
        /// <param name="parameters">The parameters which participate in the filter</param>
        public DbFilter(string whereClause, TDbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(whereClause));

            WhereClause = whereClause;
            Parameters = parameters;
        }
    }
}