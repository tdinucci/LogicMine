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
