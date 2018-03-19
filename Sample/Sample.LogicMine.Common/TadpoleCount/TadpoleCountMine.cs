using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.GetSingle;

namespace Sample.LogicMine.Common.TadpoleCount
{
  /// <summary>
  /// This inherits the security layer from AppMine and introduces a single shaft (IGetSingleShaft).
  /// 
  /// Since there is only a single shaft, even if the FrogCountTerminalLayer did support multiple shaft 
  /// types they could not be accessed using this mine.
  /// </summary>
  public class TadpoleCountMine : AppMine<Types.TadpoleCount, TadpoleCountTerminalLayer>,
    IGetSingleShaft<Types.TadpoleCount>
  {
    public TadpoleCountMine(string user, string connectionString, ITraceExporter traceExporter) :
      base(user, new TadpoleCountTerminalLayer(connectionString), traceExporter)
    {
    }

    public Task<IGetSingleBasket<Types.TadpoleCount>> SendAsync(IGetSingleBasket<Types.TadpoleCount> basket)
    {
      return new Shaft<IGetSingleBasket<Types.TadpoleCount>>(TraceExporter, TerminalLayer,
        GetStations<IGetSingleBasket<Types.TadpoleCount>>()).SendAsync(basket);
    }
  }
}
