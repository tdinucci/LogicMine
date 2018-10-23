using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.Tadpole;

namespace Sample.LogicMine.Common.MatingEvent
{
  /// <summary>
  /// This is the terminal layer for the MatingEventMine, it has only a "Post" terminal and this mean 
  /// the mine can only support a "Post" shaft.
  /// </summary>
  public class MatingEventTerminalLayer : ITerminalLayer<Types.MatingEvent>,
    IPostTerminal<IPostBasket<Types.MatingEvent, string>>
  {
    private static readonly Dictionary<string, int> FamilyEventsEvents = new Dictionary<string, int>();

    private readonly FrogMine _frogMine;
    private readonly TadpoleMine _tadpoleMine;

    public MatingEventTerminalLayer(FrogMine frogMine, TadpoleMine tadpoleMine)
    {
      _frogMine = frogMine ?? throw new ArgumentNullException(nameof(frogMine));
      _tadpoleMine = tadpoleMine ?? throw new ArgumentNullException(nameof(tadpoleMine));
    }

    public Task AddResultAsync(IPostBasket<Types.MatingEvent, string> basket, IVisit visit)
    {
      // get what we need out of the basket so that a response can be generated
      var male = basket.DescentPayload.Male;
      var female = basket.DescentPayload.Female;
      var date = basket.DescentPayload.Date;

      // we're going to patch the male and female, so there is a record of them mating
      var modifiedProps = new Dictionary<string, object> {{nameof(Types.Frog.DateLastMated), date}};

      var patchMaleBasket = new PatchBasket<int, Types.Frog, int>(
        new PatchRequest<int, Types.Frog>(new Delta<int, Types.Frog>(male.Id, modifiedProps)), basket);

      var patchFemaleBaset = new PatchBasket<int, Types.Frog, int>(
        new PatchRequest<int, Types.Frog>(new Delta<int, Types.Frog>(female.Id, modifiedProps)), basket);

      // generate some children for the male and female
      var tadpoleBaskets = GenerateTadpoles(basket, female, male, date);

      Task.WaitAll(_frogMine.SendAsync(patchMaleBasket), _frogMine.SendAsync(patchFemaleBaset));

      var tadpoleCount = tadpoleBaskets.Select(t => t.AscentPayload).Count();

      // the result of this operation is a string
      basket.AscentPayload = $"[{date:d}] {male.Name} and {female.Name} had {tadpoleCount} children";

      return Task.CompletedTask;
    }

    private IPostBasket<Types.Tadpole, int>[] GenerateTadpoles(IBasket parent,
      Types.Frog mother, Types.Frog father, DateTime date)
    {
      var tadpoleCount = 10;
      var tasks = new Task[tadpoleCount];
      var baskets = new IPostBasket<Types.Tadpole, int>[tadpoleCount];

      // generate 10 tadpoles for the mother and father
      var surname = $"{father.Id}-{mother.Id}";
      var eventNo = 1;

      // this is just something to ensure the child has a unique name - it's not important to understand
      if (FamilyEventsEvents.ContainsKey(surname))
        eventNo = FamilyEventsEvents[surname] + 1;

      for (var i = 0; i < tadpoleCount; i++)
      {
        var tadpole = new Types.Tadpole
        {
          DateOfBirth = date.AddDays(i),
          MotherId = mother.Id,
          FatherId = father.Id,
          LivesInPondId = mother.LivesInPondId,
          IsMale = i % 2 == 0,
          Name = (i % 2 == 0 ? "M" : "F") + $"{i}:{eventNo}:{surname}"
        };

        var postBasket = new PostBasket<Types.Tadpole, int>(tadpole, parent);

        tasks[i] = _tadpoleMine.SendAsync(postBasket);
        baskets[i] = postBasket;
      }

      FamilyEventsEvents[surname] = eventNo;

      Task.WaitAll(tasks);
      return baskets;
    }
  }
}