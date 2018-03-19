using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Web;
using Microsoft.AspNetCore.Mvc;
using Sample.LogicMine.Common;
using Sample.LogicMine.Common.FrogCount;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.WebApi.Controllers
{
  /// <summary>
  /// A basic controller that offers just one operation
  /// </summary>
  public class FrogCountsController : Controller,
    IGetSingleHandler
  {
    private readonly FrogCountMine _mine;

    public FrogCountsController(IAuthTokenReader tokenReader, ITraceExporter traceExporter)
    {
      _mine = new FrogCountMine(tokenReader.GetToken(), DbUtils.ConnectionString, traceExporter);
    }

    [HttpGet("[controller]/single")]
    public Task<IActionResult> GetSingleAsync([FromQuery] string filter)
    {
      return new GetSingleHandler<FrogCount>(_mine).GetSingleAsync(filter);
    }
  }
}
