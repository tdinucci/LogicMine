using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Delete;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;
using Test.LogicMine.Common.Types;

namespace Test.LogicMine.Common.Layers
{
  public class SimpleAltFrogTerminalLayer : ITerminalLayer<AltFrog>,
    IGetTerminal<IGetBasket<int, AltFrog>>,
    IGetCollectionTerminal<IGetCollectionBasket<AltFrog>>,
    IPostTerminal<IPostBasket<AltFrog, int>>,
    IPutTerminal<IPutBasket<int, AltFrog, int>>,
    IPatchTerminal<IPatchBasket<int, AltFrog, int>>,
    IDeleteTerminal<IDeleteBasket<int, int>>
  {
    public Task AddResultAsync(IGetBasket<int, AltFrog> basket, IVisit visit)
    {
      basket.AscentPayload = new AltFrog
      {
        FrogId = basket.DescentPayload,
        FrogName = "Kermit",
        FrogDateOfBirth = DateTime.Today.AddDays(-30)
      };

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IGetCollectionBasket<AltFrog> basket, IVisit visit)
    {
      basket.AscentPayload = new[]
      {
        new AltFrog {FrogId = 1, FrogName = "Kermit", FrogDateOfBirth = DateTime.Today.AddDays(-30)},
        new AltFrog {FrogId = 2, FrogName = "Frank", FrogDateOfBirth = DateTime.Today.AddDays(-29)},
        new AltFrog {FrogId = 3, FrogName = "Freddy", FrogDateOfBirth = DateTime.Today.AddDays(-28)}
      };

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IPostBasket<AltFrog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 8;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IPutBasket<int, AltFrog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 9;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IPatchBasket<int, AltFrog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 10;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IDeleteBasket<int, int> basket, IVisit visit)
    {
      basket.AscentPayload = 11;

      return Task.CompletedTask;
    }
  }
}
