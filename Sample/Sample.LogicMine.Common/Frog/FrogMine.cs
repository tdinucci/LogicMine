using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Data;
using LogicMine.Api.Data.Sqlite;

namespace Sample.LogicMine.Common.Frog
{
  /// <summary>
  /// A fairly typical mine for simple objects which aren't operated on by any business logic
  /// </summary>
  public class FrogMine : AppDataMine<int, Types.Frog>
  {
    private static SqliteMappedLayer<int, Types.Frog> GetTerminalLayer(string connectionString)
    {
      // A "descriptor" describes how the type relates to the database.  Here things are straightforward 
      // however you can create your own descriptor subclasses when the mapping is more involved, e.g. 
      // column and property names don't match.
      var descriptor = new SqliteMappedObjectDescriptor<Types.Frog>("Frog", "Id", nameof(Types.Frog.Id));
      return new SqliteMappedLayer<int, Types.Frog>(connectionString, descriptor, new DbMapper<Types.Frog>(descriptor));
    }

    public FrogMine(string user, ICache cache, string connectionString, ITraceExporter traceExporter) :
      base(user, cache, GetTerminalLayer(connectionString), traceExporter)
    {
    }
  }
}
