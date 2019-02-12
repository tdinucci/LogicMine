using System;
using System.Data;
using System.Linq;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// Represents a database statement
    /// </summary>
    /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
    public class DbStatement<TDbParameter> : IDbStatement<TDbParameter> where TDbParameter : IDbDataParameter
    {
        /// <inheritdoc />
        public string Text { get; }

        /// <inheritdoc />
        public CommandType Type { get; }

        /// <inheritdoc />
        public TDbParameter[] Parameters { get; }

        /// <summary>
        /// Construct a new DbStatement
        /// </summary>
        /// <param name="text">The statement text, i.e. SQL</param>
        /// <param name="type">The type of statement</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public DbStatement(string text, CommandType type, params TDbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

            Text = text;
            Type = type;
            Parameters = parameters ?? new TDbParameter[0];
        }

        /// <summary>
        /// Construct a new DbStatement
        /// </summary>
        /// <param name="text">The statement text, i.e. SQL</param>
        /// <param name="type">The type of statement</param>
        public DbStatement(string text, CommandType type) : this(text, type, null)
        {
        }

        /// <summary>
        /// Construct a new DbStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement text, i.e. SQL</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public DbStatement(string text, params TDbParameter[] parameters) : this(text, CommandType.Text, parameters)
        {
        }

        /// <summary>
        /// Construct a new DbStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement text, i.e. SQL</param>
        public DbStatement(string text) : this(text, CommandType.Text, null)
        {
        }

        /// <summary>
        /// Get a string representation of the statement
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{Type}] {Text} ({ParametersToString()})";
        }

        /// <summary>
        /// Get a string representation of the parameters
        /// </summary>
        /// <returns></returns>
        protected virtual string ParametersToString()
        {
            if (Parameters != null && Parameters.Any())
                return Parameters.Select(p => $"{p.ParameterName} = [{p.Value}]").Aggregate((c, n) => $"{c},{n}");

            return string.Empty;
        }
    }
}