using System;
using System.Threading.Tasks;
using LogicMine.Routing;
using Microsoft.AspNetCore.Mvc;

namespace LogicMine.Web
{
    /// <summary>
    /// A Web API controller which accepts raw requests.  These raw requests will then
    /// be dispatched to the injected IRequestRouter and this will in turn ensure they are parsed and
    /// passed to the appropriate mine and shaft so that a response may be obtained.
    /// </summary>
    public abstract class RequestController<TRawRequest> : Controller
    {
        private readonly IRequestRouter<TRawRequest> _requestRouter;
        private readonly IErrorExporter _errorExporter;

        protected RequestController(IRequestRouter<TRawRequest> requestRouter, IErrorExporter errorExporter)
        {
            _requestRouter = requestRouter ?? throw new ArgumentNullException(nameof(requestRouter));
            _errorExporter = errorExporter ?? throw new ArgumentNullException(nameof(errorExporter));
        }

        protected abstract IActionResult GetActionResult(IResponse response);

        [HttpPost]
        public async Task<IActionResult> PostRequest([FromBody] TRawRequest request)
        {
            try
            {
                var response = await _requestRouter.RouteAsync(request).ConfigureAwait(false);
                return GetActionResult(response);
            }
            catch (Exception ex)
            {
                _errorExporter.ExportError(ex);
                return new InternalServerErrorObjectResult(ex.Message);
            }
        }
    }
}