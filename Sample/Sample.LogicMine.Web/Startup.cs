using LogicMine;
using LogicMine.DataObject;
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
using Sample.LogicMine.Web.Mine.MyContact;

namespace Sample.LogicMine.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var descriptorRegistry = GetDescriptorRegistry();
            var traceExporter = new DefaultTraceExporter();
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ITraceExporter>(traceExporter)
                .AddSingleton(MineFactory.Create(descriptorRegistry, traceExporter))
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
            return new DataObjectDescriptorRegistry()
                .Register(new MyContactObjectDescriptor());
        }
    }
}