using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Filter;
using LogicMine.Api.GetSingle;
using LogicMine.Api.Post;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.FrogCount;
using Sample.LogicMine.Common.MatingEvent;
using Sample.LogicMine.Common.MaturationEvent;
using Sample.LogicMine.Common.PairSelection;
using Sample.LogicMine.Common.Pond;
using Sample.LogicMine.Common.Tadpole;
using Sample.LogicMine.Common.TadpoleCount;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.Console
{
  /// <summary>
  /// Completely useless and contrived but the hope is that it's easy to follow while 
  /// at the same time not being so trivial that it's pointless.
  /// </summary>
  public class Genesis
  {
    private DateTime _day;

    // We have a mine per data type - you may choose to follow something like the factory pattern 
    // so that you don't have to explicity "new up" mines throughout your application
    private readonly PondMine _pondMine;
    private readonly FrogMine _frogMine;
    private readonly PairSelectionMine _pairSelectionMine;
    private readonly MaturationEventMine _maturationEventMine;
    private readonly MatingEventMine _matingEventMine;
    private readonly FrogCountMine _frogCountMine;
    private readonly TadpoleCountMine _tadpoleCountMine;

    public Genesis(string user, string connectionString, ITraceExporter traceExporter)
    {
      _day = DateTime.MinValue;
      var cache = new InProcessCache();

      var tadpoleMine = new TadpoleMine(user, cache, connectionString, traceExporter);

      _pondMine = new PondMine(user, cache, connectionString, traceExporter);
      _frogMine = new FrogMine(user, cache, connectionString, traceExporter);
      _pairSelectionMine = new PairSelectionMine(user, _frogMine, traceExporter);
      _maturationEventMine = new MaturationEventMine(user, _frogMine, tadpoleMine, traceExporter);
      _matingEventMine = new MatingEventMine(user, _frogMine, tadpoleMine, traceExporter);
      _frogCountMine = new FrogCountMine(user, connectionString, traceExporter);
      _tadpoleCountMine = new TadpoleCountMine(user, connectionString, traceExporter);
    }

    /// <summary>
    /// Creates a frog universe, i.e. a pond populated with a pair of original frogs.  
    /// These frogs then go forward and multiply, having tadpoles.  These tadpoles will at 
    /// some point mature and then also breed over the space of a year.
    /// </summary>
    /// <returns></returns>
    public async Task LetThereBeLightAsync()
    {
      var pond = await CreatePondAsync();
      _day = _day.AddDays(7);

      Inhabit(pond);
      _day = _day.AddMonths(3);

      do
      {
        await Multiply();

        _day = _day.AddDays(7);
      } while (_day < DateTime.MinValue.AddYears(1));

      PrintOrganismCounts();
    }

    private async Task<Pond> CreatePondAsync()
    {
      var frogden = new Pond {Name = "Frogden"};

      // the basket that's returned is the same basket that was passed in, so if we had a reference to the original 
      // basket we could continue to refer back to this - there are examples of this later
      var basket = await _pondMine.SendAsync(new PostBasket<Pond, int>(frogden)).ConfigureAwait(false);
      frogden.Id = basket.AscentPayload;

      return frogden;
    }

    private void Inhabit(Pond pond)
    {
      var frogam = new Frog {Name = "Frogam", IsMale = true, LivesInPondId = pond.Id, DateOfBirth = _day};
      var freve = new Frog {Name = "Freve", IsMale = false, LivesInPondId = pond.Id, DateOfBirth = _day};

      Task.WaitAll(
        _frogMine.SendAsync(new PostBasket<Frog, int>(frogam)),
        _frogMine.SendAsync(new PostBasket<Frog, int>(freve))
      );
    }

    private async Task Multiply()
    {
      await MatureTadpolesAsync().ConfigureAwait(false);

      var request = new GetSingleRequest<PairSelection>(new Filter<PairSelection>(new[]
      {
        new FilterTerm(nameof(PairSelection.Date), FilterOperators.Equal, _day)
      }));

      var getPairBasket = await _pairSelectionMine.SendAsync(new GetSingleBasket<PairSelection>(request))
        .ConfigureAwait(false);

      var pair = getPairBasket.AscentPayload;
      if (pair.IsValid)
      {
        var mateBasket = new PostBasket<MatingEvent, string>(new MatingEvent(pair.Male, pair.Female, pair.Date));
       
        // this call returns a basket, but it's just the same basket that was passed in.  Since we already have a reference
        // to it we don't need to assign the result to a new variable
        await _matingEventMine.SendAsync(mateBasket).ConfigureAwait(false);

        System.Console.WriteLine(mateBasket.AscentPayload);
      }
    }

    private async Task MatureTadpolesAsync()
    {
      var maturationBasket = new PostBasket<MaturationEvent, int>(new MaturationEvent(_day));
      await _maturationEventMine.SendAsync(maturationBasket).ConfigureAwait(false);

      var maturedCount = maturationBasket.AscentPayload;
      if (maturedCount > 0)
        System.Console.WriteLine($"[{_day:d}] {maturedCount} tadpoles have matured");
    }

    private void PrintOrganismCounts()
    {
      var frogCountBasket = new GetSingleBasket<FrogCount>();
      var tadpoleCountBasket = new GetSingleBasket<TadpoleCount>();
      var tasks = new Task[]
      {
        _frogCountMine.SendAsync(frogCountBasket),
        _tadpoleCountMine.SendAsync(tadpoleCountBasket)
      };

      Task.WaitAll(tasks);

      System.Console.WriteLine(
        $"There are {frogCountBasket.AscentPayload.Count} frogs and {tadpoleCountBasket.AscentPayload.Count} tadpoles");
    }
  }
}
