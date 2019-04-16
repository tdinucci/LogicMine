using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using LogicMine;
using LogicMine.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Security
{
    public class MyIntelligentJsonRequestRouter : IntelligentJsonRequestRouter
    {
        private const string AuthorisationHeaderName = "Authorization";
        private const string AuthorisationSchemeName = "MyScheme";
        private const string AccessTokenOption = "AccessToken";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyIntelligentJsonRequestRouter(Assembly serviceAssembly, IServiceCollection serviceCollection,
            IHttpContextAccessor httpContextAccessor, ITraceExporter traceExporter = null) :
            base(serviceAssembly, serviceCollection, traceExporter)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// This method is called after a request has been parsed but before it's been dispatched to a mine.
        /// The implementation here pulls the Authorization header from HTTP request and adds it to the IRequest.  This
        /// can then be inspected within the mine and the request rejected if the access token is invalid.
        /// </summary>
        /// <param name="request">The parsed request</param>
        protected override void PreprocessRequest(IRequest request)
        {
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers[AuthorisationHeaderName].FirstOrDefault();
            var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
            if (authHeaderVal.Scheme.Equals(AuthorisationSchemeName, StringComparison.OrdinalIgnoreCase))
            {
                var accessToken = Uri.UnescapeDataString(authHeaderVal.Parameter);
                request.Options.Add(AccessTokenOption, accessToken);
            }
        }
    }
}