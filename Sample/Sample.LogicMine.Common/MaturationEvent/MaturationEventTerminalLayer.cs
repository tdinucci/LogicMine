using System;
using System.Threading.Tasks;
using System.Transactions;
using LogicMine;
using LogicMine.Api.Delete;
using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Post;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.Tadpole;

namespace Sample.LogicMine.Common.MaturationEvent
{
  /// <summary>
  /// This is the terminal layer for the MaturationEventMine, it has only a "Post" terminal and this mean 
  /// the mine can only support a "Post" shaft.
  /// </summary>
  public class MaturationEventTerminalLayer : ITerminalLayer<Types.MaturationEvent>,
    IPostTerminal<IPostBasket<Types.MaturationEvent, int>>
  {
    private readonly FrogMine _frogMine;
    private readonly TadpoleMine _tadpoleMine;

    public MaturationEventTerminalLayer(FrogMine frogMine, TadpoleMine tadpoleMine)
    {
      _frogMine = frogMine ?? throw new ArgumentNullException(nameof(frogMine));
      _tadpoleMine = tadpoleMine ?? throw new ArgumentNullException(nameof(tadpoleMine));
    }

    public async Task AddResultAsync(IPostBasket<Types.MaturationEvent, int> basket, IVisit visit)
    {
      var date = basket.DescentPayload.Date;
      var filter = new Filter<Types.Tadpole>(new[]
      {
        new FilterTerm(nameof(Types.Tadpole.DateOfBirth), FilterOperators.LessThanOrEqual, date.AddMonths(-2))
      });

      // get all tadpoles who are of a certain age - see filter above
      var getTadpolesBasket =
        new GetCollectionBasket<Types.Tadpole>(new GetCollectionRequest<Types.Tadpole>(filter));

      await _tadpoleMine.SendAsync(getTadpolesBasket).ConfigureAwait(false);

      var tadpoles = getTadpolesBasket.AscentPayload;
      if (tadpoles.Length > 0)
      {
        // covert the retrieved tadpoles into frogs...
        foreach (var tadpole in tadpoles)
        {
          var frog = tadpole.ToFrog();
          using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
          {
            // ... create frog
            var postFrogBasket = new PostBasket<Types.Frog, int>(frog);
            var postFrogTask = _frogMine.SendAsync(postFrogBasket);

            // ...and get rid of the tapole which has now become a frog
            var deleteTadpoleBasket = new DeleteBasket<int, Types.Tadpole, int>(tadpole.Id);
            var deleteTadpoleTask = _tadpoleMine.SendAsync(deleteTadpoleBasket);

            Task.WaitAll(postFrogTask, deleteTadpoleTask);

            scope.Complete();
          }
        }
      }

      basket.AscentPayload = tadpoles.Length;
    }
  }
}