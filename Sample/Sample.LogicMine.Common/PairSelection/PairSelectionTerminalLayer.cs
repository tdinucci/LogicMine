using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using LogicMine.Api.GetSingle;
using Sample.LogicMine.Common.Frog;

namespace Sample.LogicMine.Common.PairSelection
{
  /// <summary>
  /// This terminal is capable of matchmaking a couple of frogs
  /// </summary>
  public class PairSelectionTerminalLayer : ITerminalLayer<Types.PairSelection>,
    IGetSingleTerminal<IGetSingleBasket<Types.PairSelection>>
  {
    private readonly FrogMine _frogMine;

    public PairSelectionTerminalLayer(FrogMine frogMine)
    {
      _frogMine = frogMine ?? throw new ArgumentNullException(nameof(frogMine));
    }

    public Task AddResultAsync(IGetSingleBasket<Types.PairSelection> basket)
    {
      var date = basket?.DescentPayload?.Filter?.Terms?.FirstOrDefault(
        t => t.PropertyName == nameof(Types.PairSelection.Date))?.Value as DateTime?;

      if (date == null)
        throw new ArgumentException($"Expected filter on '{nameof(Types.PairSelection.Date)}'");

      // only consider males which haven't mated in the past 3 months
      var maleFilter = new Filter<Types.Frog>(new[]
      {
        new FilterTerm(nameof(Types.Frog.IsMale), FilterOperators.Equal, true),
        new FilterTerm(nameof(Types.Frog.DateLastMated), FilterOperators.LessThan, date.Value.AddMonths(-3))
      });

      // ... and the same for females
      var femaleFilter = new Filter<Types.Frog>(new[]
      {
        new FilterTerm(nameof(Types.Frog.IsMale), FilterOperators.Equal, false),
        new FilterTerm(nameof(Types.Frog.DateLastMated), FilterOperators.LessThan, date.Value.AddMonths(-3))
      });

      // Try and find a suitable pair.
      // In a real application you would most likely have a single efficient DB query.  Here I just want to 
      // demonstrate operating across different mines.
      // Note that the main basket is pased into these "child" baskets.  This is optional however doing this allows 
      // for the baskets to be linked together when exporting tracing information (see FileTraceExporter)
      var getMaleBasket =
        new GetCollectionBasket<Types.Frog>(new GetCollectionRequest<Types.Frog>(maleFilter, 1, 0), basket);
      var getMaleTask = _frogMine.SendAsync(getMaleBasket);

      var getFemaleBasket =
        new GetCollectionBasket<Types.Frog>(new GetCollectionRequest<Types.Frog>(femaleFilter, 1, 0), basket);
      var getFemaleTask = _frogMine.SendAsync(getFemaleBasket);

      Task.WaitAll(getMaleTask, getFemaleTask);

      Types.Frog male = null;
      if (getMaleBasket.AscentPayload.Length > 0)
        male = getMaleBasket.AscentPayload[0];

      Types.Frog female = null;
      if (getFemaleBasket.AscentPayload.Length > 0)
        female = getFemaleBasket.AscentPayload[0];

      basket.AscentPayload = new Types.PairSelection(male, female, date.Value);

      return Task.CompletedTask;
    }
  }
}
