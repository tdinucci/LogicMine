using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Data;
using LogicMine.Api.Data.Sqlite;

namespace Sample.LogicMine.Common.Pond
{
  /// <summary>
  /// A fairly typical mine for simple objects which aren't operated on by any business logic
  /// </summary>
  public class PondMine : AppDataMine<int, Types.Pond>
  {
    private static SqliteMappedLayer<int, Types.Pond> GetTerminalLayer(string connectionString)
    {
      // A "descriptor" describes how the type relates to the database.  Here things are straightforward 
      // however you can create your own descriptor subclasses when the mapping is more involved, e.g. 
      // column and property names don't match.
      var descriptor = new SqliteMappedObjectDescriptor<Types.Pond>("Pond", "Id", nameof(Types.Pond.Id));
      return new SqliteMappedLayer<int, Types.Pond>(connectionString, descriptor, new DbMapper<Types.Pond>(descriptor));
    }

    public PondMine(string user, ICache cache, string connectionString, ITraceExporter traceExporter) :
      base(user, cache, GetTerminalLayer(connectionString), traceExporter)
    {
    }
  }
}
