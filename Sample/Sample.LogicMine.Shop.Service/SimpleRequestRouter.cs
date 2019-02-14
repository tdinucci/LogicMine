using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.Routing.Json;
using Microsoft.AspNetCore.Http;
using Sample.LogicMine.Shop.Service.Mine.Customer;
using Sample.LogicMine.Shop.Service.Mine.Login;
using Sample.LogicMine.Shop.Service.Mine.Product;
using Sample.LogicMine.Shop.Service.Mine.Purchase;
using Sample.LogicMine.Shop.Service.Mine.SalesSummary;

namespace Sample.LogicMine.Shop.Service
{
    /// <summary>
    /// This is one of the two implementations of JsonRequestRouter in this project.  Only one is active at any time
    /// and you can choose in Startup.cs which one to use.
    ///
    /// This implementation is the simplest of the two however will need to be modified every time new functionality
    /// is added to the service.
    /// </summary>
    public class SimpleRequestRouter: JsonRequestRouter
    {
        private const string AuthorisationHeaderName = "Authorization";
        private const string BearerSchemeName = "Bearer";
        private const string AccessTokenOption = "AccessToken";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITraceExporter _traceExporter;
        private readonly DbConnectionString _dbConnectionString;

        public SimpleRequestRouter(IHttpContextAccessor httpContextAccessor, ITraceExporter traceExporter, 
            DbConnectionString dbConnectionString) : 
            base(new global::LogicMine.Mine(traceExporter), traceExporter)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _traceExporter = traceExporter ?? throw new ArgumentNullException(nameof(traceExporter));
            _dbConnectionString = dbConnectionString ?? throw new ArgumentNullException(nameof(dbConnectionString));
        }

        // This returns the custom request types the service deals with
        protected override IEnumerable<Type> GetCustomRequestTypes()
        {
            return new[]
            {
                typeof(LoginRequest),
                typeof(SalesSummaryRequest)
            };
        }
        
        // This returns the collection of data object descriptors applicable to the service
        protected override IEnumerable<IDataObjectDescriptor> GetDataObjectDescriptors()
        {
            return new IDataObjectDescriptor[]
            {
                new CustomerDescriptor(),
                new ProductDescriptor(),
                new PurchaseDescriptor()
            };
        }

        // This returns all required shaft registrars 
        protected override IEnumerable<IShaftRegistrar> GetShaftRegistrars()
        {
            return new IShaftRegistrar[]
            {
                new LoginShaftRegistrar(_traceExporter), 
                new CustomerDataObjectShaftRegistrar(_dbConnectionString, _traceExporter),
                new ProductDataObjectShaftRegistrar(_dbConnectionString, _traceExporter),
                new PurchaseDataObjectShaftRegistrar(_dbConnectionString, _traceExporter),
                new SalesSummaryShaftRegistrar(_dbConnectionString, _traceExporter)
            };
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