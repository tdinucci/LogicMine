using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace LogicMine.Api.Data.Postgres
{
    /// <summary>
    /// Represents a database statement which targets Postgres
    /// </summary>
    public class PostgresStatement : DbStatement<NpgsqlParameter>
    {
        /// <summary>
        /// Construct a new PostgresStatement
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="type">The type of statement</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public PostgresStatement(string text, CommandType type, params NpgsqlParameter[] parameters) :
            base(text, type, parameters)
        {
        }

        /// <summary>
        /// Construct a new PostgresStatement
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="type">The type of statement</param>
        public PostgresStatement(string text, CommandType type) : base(text, type)
        {
        }

        /// <summary>
        /// Construct a new PostgresStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public PostgresStatement(string text, params NpgsqlParameter[] parameters) : base(text, parameters)
        {
        }

        /// <summary>
        /// Construct a new PostgresStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement SQL</param>
        public PostgresStatement(string text) : base(text)
        {
        }
    }
}
