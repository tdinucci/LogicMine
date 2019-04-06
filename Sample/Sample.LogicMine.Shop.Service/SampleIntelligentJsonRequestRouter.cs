using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using LogicMine;
using LogicMine.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.LogicMine.Shop.Service
{
    /// <summary>
    /// This is one of the two implementations of JsonRequestRouter in this project.  Only one is active at any time
    /// and you can choose in Startup.cs which one to use.
    ///
    /// This implementation will automatically adapt to changes to the project.  This means that this class shouldn't
    /// need altered as new functionality is added to the service.
    /// </summary>
    public class SampleIntelligentJsonRequestRouter : IntelligentJsonRequestRouter
    {
        private const string AuthorisationHeaderName = "Authorization";
        private const string BearerSchemeName = "Bearer";
        private const string AccessTokenOption = "AccessToken";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public SampleIntelligentJsonRequestRouter(IServiceCollection serviceCollection,
            IHttpContextAccessor httpContextAccessor, ITraceExporter traceExporter) :
            base(Assembly.GetExecutingAssembly(), serviceCollection, traceExporter)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// This method is called after a request has been parsed but before it's been dispatched to a mine.
        /// The implementation here pulls the Authorization header from HTTP request and adds it to the IRequest.  This
        /// can then be inspected within the mine and the request rejected if the access token is invalid.
        ///
        /// The SecurityStation type in this sample makes use of the data added to the request here.
        /// </summary>
        /// <param name="request">The parsed request</param>
        protected override void PreprocessRequest(IRequest request)
        {
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers[AuthorisationHeaderName].FirstOrDefault();
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
                if (authHeaderVal.Scheme.Equals(BearerSchemeName, StringComparison.OrdinalIgnoreCase))
                {
                    var accessToken = Uri.UnescapeDataString(authHeaderVal.Parameter);
                    request.Options.Add(AccessTokenOption, accessToken);
                }
            }
        }
    }
}