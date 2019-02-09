using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;
using LogicMine.Web.Request;
using LogicMine.Web.Request.Json;
using LogicMine.Web.Request.Json.DataObject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sample.LogicMine.Web.Mine;
using Sample.LogicMine.Web.Mine.GetTime;

namespace Sample.LogicMine.Web
{
    public class Startup
    {
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";
        private const string SfAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";

        public void ConfigureServices(IServiceCollection services)
        {
            var sfConnConfig = GetSalesforceConnectionConfig();
            var descriptorRegistry = GetDescriptorRegistry();
            var traceExporter = new DefaultTraceExporter();
            var shaftRegistrars = GetShaftRegistrars(sfConnConfig, descriptorRegistry, traceExporter);

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ITraceExporter>(traceExporter)
                .AddSingleton(new MineFactory().Create(shaftRegistrars))
                .AddSingleton(CreateRequestParserRegistry(descriptorRegistry))
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private IRequestParserRegistry<JObject> CreateRequestParserRegistry(
            IDataObjectDescriptorRegistry descriptorRegistry)
        {
            return new JsonRequestParserRegistry()
                .Register(new NonGenericJsonRequestParser(typeof(GetTimeRequest)))
                .Register(new GetObjectRequestJsonParser(descriptorRegistry))
                .Register(new GetCollectionRequestJsonParser(descriptorRegistry))
                .Register(new CreateObjectRequestJsonParser(descriptorRegistry))
                .Register(new UpdateObjectRequestJsonParser(descriptorRegistry))
                .Register(new DeleteObjectRequestJsonParser(descriptorRegistry));
        }

        private IDataObjectDescriptorRegistry GetDescriptorRegistry()
        {
            var registry = new DataObjectDescriptorRegistry();
            var descriptorTypes = Assembly.GetCallingAssembly().GetTypes()
                .Where(t => typeof(IDataObjectDescriptor).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var descriptorType in descriptorTypes)
            {
                var descriptor = (IDataObjectDescriptor) Activator.CreateInstance(descriptorType);
                registry.Register(descriptor);
            }

            return registry;
        }

        private SalesforceConnectionConfig GetSalesforceConnectionConfig()
        {
            return new SalesforceConnectionConfig(SfClientId, SfClientSecret, SfUsername, SfPassword, SfAuthEndpoint);
        }

        private IEnumerable<IShaftRegistrar> GetShaftRegistrars(SalesforceConnectionConfig salesforceConnectionConfig,
            IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter)
        {
            var registrars = new List<IShaftRegistrar>();
            var registrarTypes = Assembly.GetCallingAssembly().GetTypes()
                .Where(t => typeof(IShaftRegistrar).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                            t.GetConstructors().Length > 0);

            foreach (var registrarType in registrarTypes)
            {
                IShaftRegistrar registrar = null;
                if (typeof(SampleShaftRegistrar).IsAssignableFrom(registrarType))
                    registrar = (IShaftRegistrar) Activator.CreateInstance(registrarType, traceExporter);
                else if (registrarType.BaseType?.GetGenericTypeDefinition() == typeof(SampleDataObjectShaftRegistrar<>))
                {
                    registrar = (IShaftRegistrar) Activator.CreateInstance(registrarType, salesforceConnectionConfig,
                        descriptorRegistry, traceExporter);
                }

                if (registrar == null)
                    throw new InvalidOperationException($"Unexpected registrar type of '{registrarType}'");

                registrars.Add(registrar);
            }

            return registrars.ToArray();
        }
    }
}