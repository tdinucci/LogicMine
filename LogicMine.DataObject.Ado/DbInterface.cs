﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace LogicMine.DataObject.Ado
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
        private readonly ITransientErrorAwareExecutor _transientErrorAwareExecutor;

        /// <summary>
        /// Construct a new DbInterface
        /// </summary>
        /// <param name="transientErrorAwareExecutor">If provided all database operations will be executed within it.  The implementation will dictate things like the retry policy</param>
        protected DbInterface(ITransientErrorAwareExecutor transientErrorAwareExecutor = null)
        {
            _transientErrorAwareExecutor = transientErrorAwareExecutor;
        }

        /// <summary>
        /// Returns a TDbCommand for the given statement
        /// </summary>
        /// <param name="statement">The statement which a database command is required for</param>
        /// <returns>A TDbCommand</returns>
        protected abstract TDbCommand GetDbCommand(IDbStatement<TDbParameter> statement);

        /// <inheritdoc />
        public virtual async Task<int> ExecuteNonQueryAsync(IDbStatement<TDbParameter> statement)
        {
            var execute = new Func<Task<int>>(async () =>
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
                    var conn = cmd?.Connection;
                    cmd?.Dispose();
                    conn?.Dispose();
                }
            });

            return _transientErrorAwareExecutor != null
                ? await _transientErrorAwareExecutor.ExecuteAsync(execute).ConfigureAwait(false)
                : await execute();
        }

        /// <inheritdoc />
        public virtual async Task<object> ExecuteScalarAsync(IDbStatement<TDbParameter> statement)
        {
            var execute = new Func<Task<object>>(async () =>
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
                    var conn = cmd?.Connection;
                    cmd?.Dispose();
                    conn?.Dispose();
                }
            });

            return _transientErrorAwareExecutor != null
                ? await _transientErrorAwareExecutor.ExecuteAsync(execute).ConfigureAwait(false)
                : await execute();
        }

        /// <inheritdoc />
        public virtual async Task<DbDataReader> GetReaderAsync(IDbStatement<TDbParameter> statement)
        {
            var execute = new Func<Task<DbDataReader>>(async () =>
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

                    return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);
                }
            });

            return _transientErrorAwareExecutor != null
                ? await _transientErrorAwareExecutor.ExecuteAsync(execute).ConfigureAwait(false)
                : await execute();
        }
    }
}