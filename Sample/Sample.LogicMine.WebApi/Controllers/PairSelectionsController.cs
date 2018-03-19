using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Web;
using Microsoft.AspNetCore.Mvc;
using Sample.LogicMine.Common;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.PairSelection;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.WebApi.Controllers
{
  /// <summary>
  /// A basic controller that offers just one operation
  /// </summary>
  public class PairSelectionsController : Controller,
    IGetSingleHandler
  {
    private readonly PairSelectionMine _mine;

    public PairSelectionsController(ICache cache, IAuthTokenReader tokenReader, ITraceExporter traceExporter)
    {
      if (tokenReader == null)
        throw new ArgumentNullException(nameof(tokenReader));

      var userToken = tokenReader.GetToken();
      var frogMine = new FrogMine(userToken, cache, DbUtils.ConnectionString, traceExporter);

      _mine = new PairSelectionMine(userToken, frogMine, traceExporter);
    }
    
    [HttpGet("[controller]/single")]
    public Task<IActionResult> GetSingleAsync([FromQuery] string filter)
    {
      return new GetSingleHandler<PairSelection>(_mine).GetSingleAsync(filter);
    }
  }
}
