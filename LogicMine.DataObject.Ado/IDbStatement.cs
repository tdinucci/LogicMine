using System.Data;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// Represents a database statement
    /// </summary>
    /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
    public interface IDbStatement<TDbParameter> where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// The statement text, i.e. SQL
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The type of statement
        /// </summary>
        CommandType Type { get; }

        /// <summary>
        /// The parameters which participate in the statement
        /// </summary>
        TDbParameter[] Parameters { get; }
    }
}