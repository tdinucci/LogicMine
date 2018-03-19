using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.GetSingle;

namespace Sample.LogicMine.Common.FrogCount
{
  /// <summary>
  /// This inherits the security layer from AppMine and introduces a single shaft (IGetSingleShaft).
  /// 
  /// Since there is only a single shaft, even if the FrogCountTerminalLayer did support multiple shaft 
  /// types they could not be accessed using this mine.
  /// </summary>
  public class FrogCountMine : AppMine<Types.FrogCount, FrogCountTerminalLayer>, IGetSingleShaft<Types.FrogCount>
  {
    public FrogCountMine(string user, string connectionString, ITraceExporter traceExporter) :
      base(user, new FrogCountTerminalLayer(connectionString), traceExporter)
    {
    }

    public Task<IGetSingleBasket<Types.FrogCount>> SendAsync(IGetSingleBasket<Types.FrogCount> basket)
    {
      return new Shaft<IGetSingleBasket<Types.FrogCount>>(TraceExporter, TerminalLayer,
        GetStations<IGetSingleBasket<Types.FrogCount>>()).SendAsync(basket);
    }
  }
}
