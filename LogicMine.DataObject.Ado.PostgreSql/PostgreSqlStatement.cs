using System.Data;
using Npgsql;

namespace LogicMine.DataObject.Ado.PostgreSql
{
    /// <summary>
    /// Represents a database statement which targets Postgres
    /// </summary>
    public class PostgreSqlStatement : DbStatement<NpgsqlParameter>
    {
        /// <summary>
        /// Construct a new PostgreSqlStatement
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="type">The type of statement</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public PostgreSqlStatement(string text, CommandType type, params NpgsqlParameter[] parameters) :
            base(text, type, parameters)
        {
        }

        /// <summary>
        /// Construct a new PostgreSqlStatement
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="type">The type of statement</param>
        public PostgreSqlStatement(string text, CommandType type) : base(text, type)
        {
        }

        /// <summary>
        /// Construct a new PostgreSqlStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public PostgreSqlStatement(string text, params NpgsqlParameter[] parameters) : base(text, parameters)
        {
        }

        /// <summary>
        /// Construct a new PostgreSqlStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement SQL</param>
        public PostgreSqlStatement(string text) : base(text)
        {
        }
    }
}