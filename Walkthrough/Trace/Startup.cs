using LogicMine;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Trace
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var traceExporter = new MyTraceExporter("MyService");
            var requestRouter = new IntelligentJsonRequestRouter(GetType().Assembly, services, traceExporter);
            
            services
                .AddSingleton(services)
                .AddSingleton<ITraceExporter>(traceExporter)
                .AddSingleton<IErrorExporter>(traceExporter)
                .AddSingleton<IRequestRouter<JObject>>(requestRouter)
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
