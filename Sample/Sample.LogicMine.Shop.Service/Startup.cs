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
        private static readonly string TraceFilePath = Path.Combine(Path.GetTempPath(), "sample-logicmine-shop.trace");
        
        public void ConfigureServices(IServiceCollection services)
        {
            var filename = Path.Combine(Path.GetTempPath(), "sample-logicmine-shop.db");
            var connectionString = new DbGenerator(filename).CreateDb();

            var traceExporter = new SimpleTraceExporter(TraceFilePath);

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IErrorExporter>(traceExporter)
                .AddSingleton<ITraceExporter>(traceExporter)
                .AddSingleton(services)
                .AddSingleton(new DbConnectionString(connectionString))
                //.AddSingleton<IRequestRouter<JObject>, IntelligentRequestRouter>()
                .AddSingleton<IRequestRouter<JObject>, SimpleRequestRouter>()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}