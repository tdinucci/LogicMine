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
using System;
using Npgsql;

namespace LogicMine.Api.Data.Postgres
{
    /// <summary>
    /// An general interface to Postgres
    /// </summary>
    public class PostgresInterface : DbInterface<NpgsqlCommand, NpgsqlParameter>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Construct a new PostgresInterface
        /// </summary>
        /// <param name="connectionString">The db's connection string</param>
        public PostgresInterface(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns a new NpgsqlCommand based on the provided statement
        /// </summary>
        /// <param name="statement">The statement to convert to an NpgsqlCommand</param>
        /// <returns>A new NpgsqlCommand</returns>
        protected override NpgsqlCommand GetDbCommand(IDbStatement<NpgsqlParameter> statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return new NpgsqlCommand(statement.Text, new NpgsqlConnection(_connectionString));
        }
    }
}
