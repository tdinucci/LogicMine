using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Post;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.Tadpole;

namespace Sample.LogicMine.Common.MatingEvent
{
  /// <summary>
  /// This mine only supports one operation, i.e. posting a new MatingEvent.  We could easily 
  /// introduce other shafts though which would allow us retreive historic events.
  /// 
  /// Since there is only a single shaft, even if the terminal layer did support multiple shaft 
  /// types only the IPostShaft can be accessed.
  /// </summary>
  public class MatingEventMine : AppMine<Types.MatingEvent, MatingEventTerminalLayer>,
    IPostShaft<Types.MatingEvent, string>
  {
    private enum Languages
    {
      English,
      Spanish,
      French
    };

    // change this value if you want basket contents to be translated
    private Languages Language = Languages.English;

    public MatingEventMine(string user, FrogMine frogMine, TadpoleMine tadpoleMine, ITraceExporter traceExporter) :
      base(user, new MatingEventTerminalLayer(frogMine, tadpoleMine), traceExporter)
    {
    }

    public Task<IPostBasket<Types.MatingEvent, string>> SendAsync(IPostBasket<Types.MatingEvent, string> basket)
    {
      var shaft = new Shaft<IPostBasket<Types.MatingEvent, string>>(TraceExporter, TerminalLayer,
        GetStations<IPostBasket<Types.MatingEvent, string>>());

      // Here, based on some "configuration" were injecting functionality into the shaft.
      // The station that is added will be the lowest station in the shaft (but above the terminal),
      // i.e. it will be below the stations from the security layer which was added in the base AppMine
      // class
      if (Language == Languages.Spanish)
        shaft.Add(new SpanishTranslationStation());
      else if (Language == Languages.French)
        shaft.Add(new FrenchTranslationStation());

      return shaft.SendAsync(basket);
    }
  }
}
