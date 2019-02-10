using System;
using System.Threading.Tasks;
using LogicMine.Routing;
using Microsoft.AspNetCore.Mvc;

namespace LogicMine.Web
{
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