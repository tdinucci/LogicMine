using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// An general interface to the databases compatible with System.Data.Common
    /// </summary>
    /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
    public interface IDbInterface<TDbParameter> where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// Executes a statement which returns no result
        /// </summary>
        /// <param name="statement">The statement to execute</param>
        /// <returns>A handle to a task to manage asynchronous execution of the statement</returns>
        Task<int> ExecuteNonQueryAsync(IDbStatement<TDbParameter> statement);

        /// <summary>
        /// Executes an operation which is expected to return a single, simple value, e.g. an integer.
        /// </summary>
        /// <param name="statement">The statement to execute</param>
        /// <returns>The value returned by the statement</returns>
        Task<object> ExecuteScalarAsync(IDbStatement<TDbParameter> statement);

        /// <summary>
        /// Executes a statement and returns a data reader.  When the reader is closed the database connection associated with it 
        /// is also closed.
        /// </summary>
        /// <param name="statement">A select statement</param>
        /// <returns>A data reader</returns>
        Task<DbDataReader> GetReaderAsync(IDbStatement<TDbParameter> statement);
    }
}