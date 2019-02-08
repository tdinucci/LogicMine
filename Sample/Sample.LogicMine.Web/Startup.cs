using LogicMine;
using LogicMine.Web.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sample.LogicMine.Web.Mine;

namespace Sample.LogicMine.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton(MineFactory.Create())
                .AddSingleton(CreateRequestParserRegistry())
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private IRequestParserRegistry<JObject> CreateRequestParserRegistry()
        {
            return new JsonRequestParserRegistry()
                .Register(new GetObjectRequestJsonParser());
        }
    }
}