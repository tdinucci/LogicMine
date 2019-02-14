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
    /// <summary>
    /// This is one of the two implementations of JsonRequestRouter in this project.  Only one is active at any time
    /// and you can choose in Startup.cs which one to use.
    ///
    /// This implementation is the more complex of the two however it will automatically adapt to changes to the
    /// project.  This means that this class shouldn't need altered as new functionality is added to the service.
    /// </summary>
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

        // This method dynamically finds all IDataObjectDescriptor's within the current assembly and constructs them.
        // It is expected that the implementations all have a default constructor - if this wasn't the case then the 
        // IServiceCollection which was passed to the constructor could be used to construct these types as is done in 
        // GetShaftRegistrars()
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

        // Here we dynamically discover the IShaftRegistrar implementations and instantiate them.
        // Shaft registrars won't typically have default constructors and so the DI contains that was passed in 
        // is used.
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

        // Discover the custom request types which have been defined in the assembly
        protected override IEnumerable<Type> GetCustomRequestTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IRequest).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();
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