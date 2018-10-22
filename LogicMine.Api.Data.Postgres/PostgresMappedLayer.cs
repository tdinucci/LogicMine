using System;
using System.Data.SqlClient;
using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using Npgsql;

namespace LogicMine.Api.Data.Postgres
{
  /// <summary>
  /// A terminal layer for dealing with T's which are mapped to a single Postgres table
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the terminals operate on</typeparam>
  public class PostgresMappedLayer<TId, T> : MappedDbLayer<TId, T, NpgsqlParameter>
    where T : new()
  {
    protected override string SafeIdentifierFormat => "\"{0}\"";
    
    /// <summary>
    /// Construct a new PostgresMappedLayer
    /// </summary>
    /// <param name="connectionString">The db's connection string</param>
    /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
    /// <param name="mapper">An object-relational mapper</param>
    public PostgresMappedLayer(string connectionString, PostgresMappedObjectDescriptor<T> descriptor,
      IDbMapper<T> mapper) :
      base(new PostgresInterface(connectionString), descriptor, mapper)
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

      return new NpgsqlParameter(name, value ?? DBNull.Value);
    }

    /// <inheritdoc />
    protected override IDbFilter<NpgsqlParameter> GetDbFilter(IFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException(nameof(filter));

      return new PostgresFilterGenerator(filter, (n) => MakeColumnNameSafe(Descriptor.GetMappedColumnName(n))).Generate();
    }

    /// <inheritdoc />
    protected override string GetSelectLastIdentityQuery()
    {
      return "SELECT CAST(lastval() AS integer);";
    }

    /// <inheritdoc />
    protected override IDbStatement<NpgsqlParameter> GetSelectDbStatement(IGetCollectionRequest<T> request)
    {
      var max = request.Max.GetValueOrDefault(0);
      if (max > 0)
      {
        if (request.Filter != null)
          return GetSelectDbStatement(request.Filter, max, request.Page.GetValueOrDefault(0));

        return GetSelectDbStatement(max, request.Page.GetValueOrDefault(0));
      }

      return base.GetSelectDbStatement(request);
    }

    private IDbStatement<NpgsqlParameter> GetSelectDbStatement(int max, int page)
    {
      var query = $"SELECT {GetSelectableColumns()} FROM {Descriptor.Table} ORDER BY {Descriptor.PrimaryKey} " +
                  $"LIMIT {max} OFFSET {max} * {page};";

      return new DbStatement<NpgsqlParameter>(query);
    }

    private IDbStatement<NpgsqlParameter> GetSelectDbStatement(IFilter<T> filter, int max, int page)
    {
      var sqlFilter = GetDbFilter(filter);

      var query = $"SELECT {GetSelectableColumns()} FROM {Descriptor.Table} " +
                  $"{sqlFilter.WhereClause} ORDER BY {Descriptor.PrimaryKey} " +
                  $"LIMIT {max} OFFSET {max} * {page};";

      return new DbStatement<NpgsqlParameter>(query, sqlFilter.Parameters);
    }
  }
}
