using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Web;
using Microsoft.AspNetCore.Mvc;
using Sample.LogicMine.Common;
using Sample.LogicMine.Common.TadpoleCount;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.WebApi.Controllers
{
  /// <summary>
  /// A basic controller that offers just one operation
  /// </summary>
  public class TadpoleCountsController : Controller,
    IGetSingleHandler
  {
    private readonly TadpoleCountMine _mine;

    public TadpoleCountsController(IAuthTokenReader tokenReader, ITraceExporter traceExporter)
    {
      _mine = new TadpoleCountMine(tokenReader.GetToken(), DbUtils.ConnectionString, traceExporter);
    }

    [HttpGet("[controller]/single")]
    public Task<IActionResult> GetSingleAsync([FromQuery] string filter)
    {
      return new GetSingleHandler<TadpoleCount>(_mine).GetSingleAsync(filter);
    }
  }
}
