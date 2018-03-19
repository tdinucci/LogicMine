using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Data;
using LogicMine.Api.Data.Sqlite;

namespace Sample.LogicMine.Common.Tadpole
{
  /// <summary>
  /// A fairly typical mine for simple objects which aren't operated on by any business logic
  /// </summary>
  public class TadpoleMine : AppDataMine<int, Types.Tadpole>
  {
    private static SqliteMappedLayer<int, Types.Tadpole> GetTerminalLayer(string connectionString)
    {
      // A "descriptor" describes how the type relates to the database.  Here things are straightforward 
      // however you can create your own descriptor subclasses when the mapping is more involved, e.g. 
      // column and property names don't match.
      var descriptor = new SqliteMappedObjectDescriptor<Types.Tadpole>("Tadpole", "Id", nameof(Types.Tadpole.Id));
      return new SqliteMappedLayer<int, Types.Tadpole>(connectionString, descriptor, new DbMapper<Types.Tadpole>(descriptor));
    }

    public TadpoleMine(string user, ICache cache, string connectionString, ITraceExporter traceExporter) :
      base(user, cache, GetTerminalLayer(connectionString), traceExporter)
    {
    }
  }
}
