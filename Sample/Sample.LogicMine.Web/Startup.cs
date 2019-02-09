using LogicMine;
using LogicMine.DataObject;
using LogicMine.Web.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sample.LogicMine.Web.Mine;
using Sample.LogicMine.Web.Mine.MyContact;

namespace Sample.LogicMine.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var descriptorRegistry = GetDescriptorRegistry();
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton(MineFactory.Create(descriptorRegistry))
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
                .Register(new GetObjectRequestJsonParser(descriptorRegistry))
                .Register(new GetCollectionRequestJsonParser(descriptorRegistry));
        }

        private IDataObjectDescriptorRegistry GetDescriptorRegistry()
        {
            return new DataObjectDescriptorRegistry()
                .Register(new MyContactObjectDescriptor());
        }
    }
}