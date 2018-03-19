/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace LogicMine.Api.Data
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
