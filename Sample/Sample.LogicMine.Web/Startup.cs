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
using Sample.LogicMine.Web.Mine.Candidate.UploadCv;
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
            var descriptorTypes = GetDataObjectDescriptorTypes();
            var shaftRegistrarTypes = GetShaftRegistrarTypes();

            foreach (var shaftRegistrarType in shaftRegistrarTypes)
                services.AddSingleton(shaftRegistrarType);

            foreach (var descriptorType in descriptorTypes)
                services.AddSingleton(descriptorType);

            services
                .AddSingleton(new SalesforceCredentials(SfClientId, SfClientSecret, SfUsername, SfPassword, SfAuthEndpoint))
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ITraceExporter>(new DefaultTraceExporter())
                .AddSingleton(p => BuildDataObjectDescriptorRegistry(p, descriptorTypes))
                .AddSingleton(p => BuildRequestParserRegistry(p.GetService<IDataObjectDescriptorRegistry>()))
                .AddSingleton(p => new MineFactory(BuildShaftRegistrars(p, shaftRegistrarTypes)).Create())
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private IRequestParserRegistry<JObject> BuildRequestParserRegistry(
            IDataObjectDescriptorRegistry descriptorRegistry)
        {
            return new JsonRequestParserRegistry()
                .Register(new NonGenericJsonRequestParser(GetCustomRequestTypes()))
                .Register(new GetObjectRequestJsonParser(descriptorRegistry))
                .Register(new GetCollectionRequestJsonParser(descriptorRegistry))
                .Register(new CreateObjectRequestJsonParser(descriptorRegistry))
                .Register(new UpdateObjectRequestJsonParser(descriptorRegistry))
                .Register(new DeleteObjectRequestJsonParser(descriptorRegistry));
        }

        private IDataObjectDescriptorRegistry BuildDataObjectDescriptorRegistry(IServiceProvider provider,
            IEnumerable<Type> dataObjectDescriptorTypes)
        {
            var registry = new DataObjectDescriptorRegistry();
            foreach (var dataObjectDescriptorType in dataObjectDescriptorTypes)
            {
                var descriptor = (IDataObjectDescriptor) provider.GetService(dataObjectDescriptorType);
                registry.Register(descriptor);
            }

            return registry;
        }

        private IEnumerable<IShaftRegistrar> BuildShaftRegistrars(IServiceProvider provider,
            IEnumerable<Type> shaftRegistrarTypes)
        {
            return shaftRegistrarTypes.Select(t => (IShaftRegistrar) provider.GetService(t));
        }

        private Type[] GetCustomRequestTypes()
        {
            return Assembly.GetCallingAssembly().GetTypes()
                .Where(t => typeof(IRequest).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();
        }

        private Type[] GetDataObjectDescriptorTypes()
        {
            return Assembly.GetCallingAssembly().GetTypes()
                .Where(t => typeof(IDataObjectDescriptor).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();
        }

        private Type[] GetShaftRegistrarTypes()
        {
            return Assembly.GetCallingAssembly().GetTypes()
                .Where(t => typeof(IShaftRegistrar).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                            t.GetConstructors().Length > 0)
                .ToArray();
        }
    }
}
