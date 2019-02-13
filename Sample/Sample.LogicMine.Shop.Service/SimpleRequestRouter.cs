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

        protected override IEnumerable<Type> GetCustomRequestTypes()
        {
            return new[]
            {
                typeof(LoginRequest),
                typeof(SalesSummaryRequest)
            };
        }
        
        protected override IEnumerable<IDataObjectDescriptor> GetDataObjectDescriptors()
        {
            return new IDataObjectDescriptor[]
            {
                new CustomerDescriptor(),
                new ProductDescriptor(),
                new PurchaseDescriptor()
            };
        }

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