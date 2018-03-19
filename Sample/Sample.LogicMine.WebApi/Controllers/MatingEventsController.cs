using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Cache;
using LogicMine.Api.Web;
using Microsoft.AspNetCore.Mvc;
using Sample.LogicMine.Common;
using Sample.LogicMine.Common.Frog;
using Sample.LogicMine.Common.MatingEvent;
using Sample.LogicMine.Common.Tadpole;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.WebApi.Controllers
{
  /// <summary>
  /// A basic controller that offers just one operation
  /// </summary>
  public class MatingEventsController : Controller,
    IPostHandler<MatingEvent>
  {
    private readonly MatingEventMine _mine;

    public MatingEventsController(ICache cache, IAuthTokenReader tokenReader, ITraceExporter traceExporter)
    {
      if (tokenReader == null)
        throw new ArgumentNullException(nameof(tokenReader));

      var userToken = tokenReader.GetToken();
      var frogMine = new FrogMine(userToken, cache, DbUtils.ConnectionString, traceExporter);
      var tadpoleMine = new TadpoleMine(userToken, cache, DbUtils.ConnectionString, traceExporter);

      _mine = new MatingEventMine(userToken, frogMine, tadpoleMine, traceExporter);
    }

    [HttpPost("[controller]")]
    public Task<IActionResult> PostAsync([FromBody] MatingEvent obj)
    {
      return new PostHandler<MatingEvent, string>(_mine).PostAsync(obj);
    }
  }
}
