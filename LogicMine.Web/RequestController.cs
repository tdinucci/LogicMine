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

        /// <summary>
        /// Construct a RequestController
        /// </summary>
        /// <param name="requestRouter">The router which takes raw requests and dispatches them to a mine</param>
        /// <param name="errorExporter">The error exporter to use when errors occur</param>
        protected RequestController(IRequestRouter<TRawRequest> requestRouter, IErrorExporter errorExporter)
        {
            _requestRouter = requestRouter ?? throw new ArgumentNullException(nameof(requestRouter));
            _errorExporter = errorExporter ?? throw new ArgumentNullException(nameof(errorExporter));
        }

        /// <summary>
        /// Returns an action result which suits the response, e.g. HTTP 200 on success and HTTP 500 on error
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        protected abstract IActionResult GetActionResult(TRawRequest request, IResponse response);

        /// <summary>
        /// An endpoint for HTTP POST requests which contain requests.  These requests will be passed to the
        /// request router (which was passed to the constructor) and this will then facilitate the parsing of the
        /// request and dispatch to a mine.
        /// </summary>
        /// <param name="request">The raw request provided in the HTTP body</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostRequest([FromBody] TRawRequest request)
        {
            try
            {
                var response = await _requestRouter.RouteAsync(request).ConfigureAwait(false);
                return GetActionResult(request, response);
            }
            catch (Exception ex)
            {
                _errorExporter.ExportError(ex);
                return new InternalServerErrorObjectResult(ex.Message);
            }
        }
    }
}