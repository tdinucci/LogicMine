using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.Routing.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.LogicMine.Shop.Service
{
    public class IntelligentRequestRouter : JsonRequestRouter
    {
        private const string AuthorisationHeaderName = "Authorization";
        private const string BearerSchemeName = "Bearer";
        private const string AccessTokenOption = "AccessToken";

        private readonly IServiceCollection _serviceCollection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IntelligentRequestRouter(IServiceCollection serviceCollection, IHttpContextAccessor httpContextAccessor,
            ITraceExporter traceExporter) : base(new global::LogicMine.Mine(traceExporter), traceExporter)
        {
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override IEnumerable<IDataObjectDescriptor> GetDataObjectDescriptors()
        {
            var descriptorTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IDataObjectDescriptor).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            var descriptors = new List<IDataObjectDescriptor>();
            foreach (var descriptorType in descriptorTypes)
            {
                try
                {
                    var descriptor = (IDataObjectDescriptor) Activator.CreateInstance(descriptorType);
                    descriptors.Add(descriptor);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to construct instance of '{descriptorType}': {ex.Message}");
                }
            }

            return descriptors;
        }

        protected override IEnumerable<IShaftRegistrar> GetShaftRegistrars()
        {
            var registrarTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IShaftRegistrar).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                            t.GetConstructors().Length > 0)
                .ToArray();

            // register types with the DI container that will be required to construct the registrars
            _serviceCollection.AddSingleton(DataObjectDescriptorRegistry);
            foreach (var registrarType in registrarTypes)
                _serviceCollection.AddSingleton(registrarType);

            var provider = _serviceCollection.BuildServiceProvider();

            return registrarTypes.Select(registrarType => (IShaftRegistrar) provider.GetService(registrarType));
        }

        protected override IEnumerable<Type> GetCustomRequestTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IRequest).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();
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