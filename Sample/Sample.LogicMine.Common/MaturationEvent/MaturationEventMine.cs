using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Post;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.Tadpole;

namespace Sample.LogicMine.Common.MaturationEvent
{
  /// <summary>
  /// A simple mine that has only a single shaft.
  /// 
  /// Since there is only a single shaft, even if the terminal layer did support multiple shaft 
  /// types only the IPostShaft can be used.
  /// </summary>
  public class MaturationEventMine : AppMine<Types.MaturationEvent, MaturationEventTerminalLayer>,
    IPostShaft<Types.MaturationEvent, int>
  {
    public MaturationEventMine(string user, FrogMine frogMine, TadpoleMine tadpoleMine, ITraceExporter traceExporter) :
      base(user, new MaturationEventTerminalLayer(frogMine, tadpoleMine), traceExporter)
    {
    }

    public Task<IPostBasket<Types.MaturationEvent, int>> SendAsync(IPostBasket<Types.MaturationEvent, int> basket)
    {
      return new Shaft<IPostBasket<Types.MaturationEvent, int>>(TraceExporter, TerminalLayer,
        GetStations<IPostBasket<Types.MaturationEvent, int>>()).SendAsync(basket);
    }
  }
}
