using System;
using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Web;
using Sample.LogicMine.Common;
using Sample.LogicMine.Common.Tadpole;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.WebApi.Controllers
{
  /// <summary>
  /// A controller that provides the standard set of API operations; Get, Post, Delete, etc.
  /// N.B. Should the underlying mine not support an operation which the GeneralResourceController does then 
  /// you will get a runtime exception if the operation is attempted
  /// </summary>
  public class TadpolesController : GeneralResourceController<int, Tadpole>
  {
    private static TadpoleMine GetMine(ICache cache, IAuthTokenReader tokenReader, ITraceExporter traceExporter)
    {
      if (tokenReader == null)
        throw new ArgumentNullException(nameof(tokenReader));

      return new TadpoleMine(tokenReader.GetToken(), cache, DbUtils.ConnectionString, traceExporter);
    }

    public TadpolesController(ICache cache, IAuthTokenReader tokenReader, ITraceExporter traceExporter) : 
      base(GetMine(cache, tokenReader, traceExporter))
    {
    }
  }
}
