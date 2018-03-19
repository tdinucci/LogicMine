using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Data;
using LogicMine.Api.Delete;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;

namespace Sample.LogicMine.Common
{
  /// <summary>
  /// This is the basis of a mine for your typical type which is backed up to a database. 
  /// 
  /// This implements a number of shafts
  /// </summary>
  public class AppDataMine<TId, T> : AppMine<T, IDbLayer<TId, T>>,
    IGetShaft<TId, T>,
    IGetCollectionShaft<T>,
    IPostShaft<T, TId>,
    IPatchShaft<TId, T, int>,
    IDeleteShaft<TId, int>
    where T : new()
  {
    /// <summary>
    /// A caching layer is introduced here and it will sit below the security layer which the base AppMine class 
    /// added.
    /// 
    /// N.B. Since this application is targetting SQLite and the database is going to be very small there is absolutely 
    /// no need for this cache, in fact it will probably actually degrade performance a litte.  This is added just to 
    /// demonstrate what you can do though.
    /// </summary>
    public AppDataMine(string user, ICache cache, IDbLayer<TId, T> terminalLayer, ITraceExporter traceExporter) :
      base(user, terminalLayer, traceExporter)
    {
      Add(new CacheLayer<TId, T>(new ObjectCache<TId, T>(cache)));
    }

    public Task<IGetBasket<TId, T>> SendAsync(IGetBasket<TId, T> basket)
    {
      return new Shaft<IGetBasket<TId, T>>(TraceExporter, TerminalLayer, GetStations<IGetBasket<TId, T>>())
        .SendAsync(basket);
    }

    public Task<IGetCollectionBasket<T>> SendAsync(IGetCollectionBasket<T> basket)
    {
      return new Shaft<IGetCollectionBasket<T>>(TraceExporter, TerminalLayer, GetStations<IGetCollectionBasket<T>>())
        .SendAsync(basket);
    }

    public Task<IPostBasket<T, TId>> SendAsync(IPostBasket<T, TId> basket)
    {
      return new Shaft<IPostBasket<T, TId>>(TraceExporter, TerminalLayer, GetStations<IPostBasket<T, TId>>())
        .SendAsync(basket);
    }

    public Task<IPatchBasket<TId, T, int>> SendAsync(IPatchBasket<TId, T, int> basket)
    {
      return new Shaft<IPatchBasket<TId, T, int>>(TraceExporter, TerminalLayer, GetStations<IPatchBasket<TId, T, int>>())
        .SendAsync(basket);
    }

    public Task<IDeleteBasket<TId, int>> SendAsync(IDeleteBasket<TId, int> basket)
    {
      return new Shaft<IDeleteBasket<TId, int>>(TraceExporter, TerminalLayer, GetStations<IDeleteBasket<TId, int>>())
        .SendAsync(basket);
    }
  }
}
