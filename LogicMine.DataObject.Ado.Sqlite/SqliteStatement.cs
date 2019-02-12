using System.Data;
using Microsoft.Data.Sqlite;

namespace LogicMine.DataObject.Ado.Sqlite
{
    /// <summary>
    /// Represents a database statement which targets SQLite
    /// </summary>
    public class SqliteStatement : DbStatement<SqliteParameter>
    {
        /// <summary>
        /// Construct a new SqliteStatement
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="type">The type of statement</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public SqliteStatement(string text, CommandType type, params SqliteParameter[] parameters) : base(text, type,
            parameters)
        {
        }

        /// <summary>
        /// Construct a new SqliteStatement
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="type">The type of statement</param>
        public SqliteStatement(string text, CommandType type) : base(text, type)
        {
        }

        /// <summary>
        /// Construct a new SqliteStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement SQL</param>
        /// <param name="parameters">The parameters which participate in the statement</param>
        public SqliteStatement(string text, params SqliteParameter[] parameters) : base(text, parameters)
        {
        }

        /// <summary>
        /// Construct a new SqliteStatement which has the default Type of CommandType.Text
        /// </summary>
        /// <param name="text">The statement SQL</param>
        public SqliteStatement(string text) : base(text)
        {
        }
    }
}