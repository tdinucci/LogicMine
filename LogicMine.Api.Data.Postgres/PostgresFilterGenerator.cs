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
using LogicMine.Api.Filter;
using Npgsql;

namespace LogicMine.Api.Data.Postgres
{
  /// <summary>
  /// Used to convert IFilters to IDbFilters suitable for use with Postgres
  /// </summary>
  public class PostgresFilterGenerator : DbFilterGenerator<NpgsqlParameter>
  {
    /// <summary>
    /// Construct a new PostgresFilterGenerator
    /// </summary>
    /// <param name="filter">The IFilter to convert</param>
    public PostgresFilterGenerator(IFilter filter) : base(filter)
    {
    }

    /// <summary>
    /// Construct a new PostgresFilterGenerator
    /// </summary>
    /// <param name="filter">The IFilter to convert</param>
    /// <param name="covertPropToFieldName">A function that can map IFilter term properties to database column names</param>
    public PostgresFilterGenerator(IFilter filter, Func<string, string> covertPropToFieldName) :
      base(filter, covertPropToFieldName)
    {
    }

    /// <summary>
    /// Get a new NpgsqlParameter based on the provided arguments
    /// </summary>
    /// <param name="name">The parameter name</param>
    /// <param name="value">The parameter value</param>
    /// <returns>A new NpgsqlParameter</returns>
    protected override NpgsqlParameter GetDbParameter(string name, object value)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

      return new NpgsqlParameter(name, value);
    }

    /// <summary>
    /// Get a new DbFilter specifically for use with Postgres
    /// </summary>
    /// <param name="clause">The WHERE clause</param>
    /// <param name="parameters">The parameters which participate in the WHERE clause</param>
    /// <returns></returns>
    protected override IDbFilter<NpgsqlParameter> GetDbFilter(string clause, NpgsqlParameter[] parameters)
    {
      if (string.IsNullOrWhiteSpace(clause))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(clause));

      return new DbFilter<NpgsqlParameter>(clause, parameters);
    }
  }
}
