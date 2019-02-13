using System.IO;
using LogicMine;
using LogicMine.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Shop.Service
{
    public class Startup
    {   
        public void ConfigureServices(IServiceCollection services)
        {
            var filename = Path.Combine(Path.GetTempPath(), "sample-logicmine-shop.db");
            var connectionString = new DbGenerator(filename).CreateDb();
            
            var traceExporter = new SampleTraceExporter();

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IErrorExporter>(traceExporter)
                .AddSingleton<ITraceExporter>(traceExporter)
                .AddSingleton(services)
                .AddSingleton(new DbConnectionString(connectionString))
                .AddSingleton<IRequestRouter<JObject>, SampleRequestRouter>()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}