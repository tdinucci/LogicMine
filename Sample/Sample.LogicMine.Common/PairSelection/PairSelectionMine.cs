using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.GetSingle;
using Sample.LogicMine.Common.Frog;

namespace Sample.LogicMine.Common.PairSelection
{
  /// <summary>
  /// This mine operates on the operational PairSelection type and just supports the "Get Single" operation.
  /// The "Get Single" operation is suitable where it should only be possible to obtain 1 (and only 1) result 
  /// from a mine.   
  /// </summary>
  public class PairSelectionMine : AppMine<Types.PairSelection, PairSelectionTerminalLayer>,
    IGetSingleShaft<Types.PairSelection>
  {
    public PairSelectionMine(string user, FrogMine frogMine, ITraceExporter traceExporter) :
      base(user, new PairSelectionTerminalLayer(frogMine), traceExporter)
    {
    }

    public Task<IGetSingleBasket<Types.PairSelection>> SendAsync(IGetSingleBasket<Types.PairSelection> basket)
    {
      return new Shaft<IGetSingleBasket<Types.PairSelection>>(TraceExporter, TerminalLayer,
          GetStations<IGetSingleBasket<Types.PairSelection>>())
        .SendAsync(basket);
    }
  }
}
