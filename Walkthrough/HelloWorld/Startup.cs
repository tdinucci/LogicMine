using LogicMine;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace HelloWorld
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var requestRouter = new IntelligentJsonRequestRouter(GetType().Assembly, services);

            services
                .AddSingleton(services)
                .AddSingleton<IRequestRouter<JObject>>(requestRouter)
                .AddSingleton<IErrorExporter, MyErrorExporter>()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
