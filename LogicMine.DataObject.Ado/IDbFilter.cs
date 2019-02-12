using System.Data;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// Represents a database query filter
    /// </summary>
    /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
    public interface IDbFilter<TDbParameter> where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// The parameters which participate in the filter
        /// </summary>
        TDbParameter[] Parameters { get; }

        /// <summary>
        /// The WHERE clause
        /// </summary>
        string WhereClause { get; }
    }
}