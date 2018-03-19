using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Data.Sqlite;
using LogicMine.Api.GetSingle;

namespace Sample.LogicMine.Common.TadpoleCount
{
  /// <summary>
  /// The TadpoleCount type isn't a concrete data object and as such we're not using an existing 
  /// data access terminal layer. We could but that would mean that we're inheriting a lot of 
  /// stuff that we don't need - at best this would mean we're in violation of the interface 
  /// segregation principle but at worst it make runtime errors more likely because there would 
  /// be entry points to functionality which we would never expect to be called.
  /// </summary>
  public class TadpoleCountTerminalLayer : ITerminalLayer<Types.TadpoleCount>, IGetSingleTerminal<IGetSingleBasket<Types.TadpoleCount>>
  {
    private readonly string _connectionString;

    public TadpoleCountTerminalLayer(string connectionString)
    {
      _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task AddResultAsync(IGetSingleBasket<Types.TadpoleCount> basket)
    {
      // since this is just a sample application and I want it to be easy to follow I'm just referencing these 
      // SQLite specific classes directly.  In a real application you may decide to introduce a level of abstraction 
      // here so that you can easily swap between database implementations later.  This could be your own abstraction 
      // or something like EntityFramework, NHibernate, etc.
      var statement = new SqliteStatement("SELECT COUNT(*) FROM Tadpole");
      var result =
        await new SqliteInterface(_connectionString).ExecuteScalarAsync(statement).ConfigureAwait(false);

      if (result == null)
        throw new SystemException("Failed to count tadpoles");

      // result will come from SQLite as a long
      basket.AscentPayload = new Types.TadpoleCount(Convert.ToInt32(result));
    }
  }
}
