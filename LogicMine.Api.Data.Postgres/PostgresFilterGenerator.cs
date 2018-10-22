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
