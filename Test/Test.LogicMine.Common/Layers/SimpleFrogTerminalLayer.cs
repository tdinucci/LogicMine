using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.GetSingle;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;
using Test.LogicMine.Common.Types;

namespace Test.LogicMine.Common.Layers
{
  public class SimpleFrogTerminalLayer : ITerminalLayer<Frog>,
    IGetTerminal<IGetBasket<int, Frog>>,
    IGetSingleTerminal<IGetSingleBasket<Frog>>,
    IGetCollectionTerminal<IGetCollectionBasket<Frog>>,
    IPostTerminal<IPostBasket<Frog,int>>,
    IPutTerminal<IPutBasket<int, Frog, int>>,
    IPatchTerminal<IPatchBasket<int, Frog, int>>,
    IDeleteTerminal<IDeleteBasket<int,int>>,
    IDeleteCollectionTerminal<IDeleteCollectionBasket<Frog,int>>
  {
    public Task AddResultAsync(IGetBasket<int, Frog> basket, IVisit visit)
    {
      basket.AscentPayload = new Frog
      {
        Id = basket.DescentPayload,
        Name = "Kermit",
        DateOfBirth = DateTime.Today.AddDays(-30)
      };

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IGetSingleBasket<Frog> basket, IVisit visit)
    {
      basket.AscentPayload = new Frog
      {
        Id = 465,
        Name = "Kermit",
        DateOfBirth = DateTime.Today.AddDays(-30)
      };

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IGetCollectionBasket<Frog> basket, IVisit visit)
    {
      basket.AscentPayload = new[]
      {
        new Frog {Id = 1, Name = "Kermit", DateOfBirth = DateTime.Today.AddDays(-30)},
        new Frog {Id = 2, Name = "Frank", DateOfBirth = DateTime.Today.AddDays(-29)},
        new Frog {Id = 3, Name = "Freddy", DateOfBirth = DateTime.Today.AddDays(-28)}
      };

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IPostBasket<Frog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 8;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IPutBasket<int, Frog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 1;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IPatchBasket<int, Frog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 1;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IDeleteBasket<int, int> basket, IVisit visit)
    {
      basket.AscentPayload = 1;

      return Task.CompletedTask;
    }

    public Task AddResultAsync(IDeleteCollectionBasket<Frog, int> basket, IVisit visit)
    {
      basket.AscentPayload = 5;

      return Task.CompletedTask;
    }
  }
}
