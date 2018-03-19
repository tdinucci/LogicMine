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
  /// <typeparam name="TDbCommand">The command type to use with the chosen database connection</typeparam>
  /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
  public abstract class DbInterface<TDbCommand, TDbParameter> : IDbInterface<TDbParameter>
    where TDbCommand : DbCommand
    where TDbParameter : IDbDataParameter
  {
    /// <summary>
    /// Returns a TDbCommand for the given statement
    /// </summary>
    /// <param name="statement">The statement which a database command is required for</param>
    /// <returns>A TDbCommand</returns>
    protected abstract TDbCommand GetDbCommand(IDbStatement<TDbParameter> statement);

    /// <inheritdoc />
    public virtual async Task<int> ExecuteNonQueryAsync(IDbStatement<TDbParameter> statement)
    {
      var cmd = GetDbCommand(statement);
      try
      {
        if (cmd.Connection.State != ConnectionState.Open)
          cmd.Connection.Open();

        cmd.CommandType = statement.Type;
        if (statement.Parameters != null)
        {
          // AddRange(...) seems to have a bug in SqliteCommand
          foreach (var parameter in statement.Parameters)
            cmd.Parameters.Add(parameter);
        }

        // await so that the connection doesn't close before the operation is finished
        return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
      }
      finally
      {
        cmd?.Dispose();
        cmd?.Connection?.Dispose();
      }
    }

    /// <inheritdoc />
    public virtual async Task<object> ExecuteScalarAsync(IDbStatement<TDbParameter> statement)
    {
      var cmd = GetDbCommand(statement);
      try
      {
        if (cmd.Connection.State != ConnectionState.Open)
          cmd.Connection.Open();

        cmd.CommandType = statement.Type;
        if (statement.Parameters != null)
        {
          // AddRange(...) seems to have a bug in SqliteCommand
          foreach (var parameter in statement.Parameters)
            cmd.Parameters.Add(parameter);
        }

        // await so that the connection doesn't close before the operation is finished
        return await cmd.ExecuteScalarAsync().ConfigureAwait(false);
      }
      finally
      {
        cmd?.Dispose();
        cmd?.Connection?.Dispose();
      }
    }

    /// <inheritdoc />
    public virtual Task<DbDataReader> GetReaderAsync(IDbStatement<TDbParameter> statement)
    {
      using (var cmd = GetDbCommand(statement))
      {
        if (cmd.Connection.State != ConnectionState.Open)
          cmd.Connection.Open();

        cmd.CommandType = statement.Type;
        if (statement.Parameters != null)
        {
          // AddRange(...) seems to have a bug in SqliteCommand
          foreach (var parameter in statement.Parameters)
            cmd.Parameters.Add(parameter);
        }

        return cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
      }
    }
  }
}
