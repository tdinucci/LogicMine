using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Data;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;

namespace Test.LogicMine.Common.Mine
{
  public class DataMine<TId, T> : Mine<T>,
    IGetShaft<TId, T>,
    IGetCollectionShaft<T>,
    IPostShaft<T, TId>,
    IPatchShaft<TId, T, int>,
    IDeleteShaft<TId, int>,
    IDeleteCollectionShaft<T, int>
    where T : new()
  {
    public new IDbLayer<TId, T> TerminalLayer => (IDbLayer<TId, T>) base.TerminalLayer;

    public DataMine(IDbLayer<TId, T> terminalLayer) : base(terminalLayer)
    {
    }

    public Task<IGetBasket<TId, T>> SendAsync(IGetBasket<TId, T> basket)
    {
      return new Shaft<IGetBasket<TId, T>>(TerminalLayer, GetStations<IGetBasket<TId, T>>()).SendAsync(basket);
    }

    public Task<IGetCollectionBasket<T>> SendAsync(IGetCollectionBasket<T> basket)
    {
      return new Shaft<IGetCollectionBasket<T>>(TerminalLayer, GetStations<IGetCollectionBasket<T>>())
        .SendAsync(basket);
    }

    public Task<IPostBasket<T, TId>> SendAsync(IPostBasket<T, TId> basket)
    {
      return new Shaft<IPostBasket<T, TId>>(TerminalLayer, GetStations<IPostBasket<T, TId>>()).SendAsync(basket);
    }

    public Task<IPatchBasket<TId, T, int>> SendAsync(IPatchBasket<TId, T, int> basket)
    {
      return new Shaft<IPatchBasket<TId, T, int>>(TerminalLayer, GetStations<IPatchBasket<TId, T, int>>())
        .SendAsync(basket);
    }

    public Task<IDeleteBasket<TId, int>> SendAsync(IDeleteBasket<TId, int> basket)
    {
      return new Shaft<IDeleteBasket<TId, int>>(TerminalLayer, GetStations<IDeleteBasket<TId, int>>())
        .SendAsync(basket);
    }

    public Task<IDeleteCollectionBasket<T, int>> SendAsync(IDeleteCollectionBasket<T, int> basket)
    {
      return new Shaft<IDeleteCollectionBasket<T, int>>(TerminalLayer, GetStations<IDeleteCollectionBasket<T, int>>())
        .SendAsync(basket);
    }
  }
}
