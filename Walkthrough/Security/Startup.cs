using LogicMine;
using LogicMine.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Security
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(services)
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IErrorExporter, MyErrorExporter>()

                .AddSingleton<IRequestRouter<JObject>>(s =>
                    new MyIntelligentJsonRequestRouter(GetType().Assembly, services,
                        s.GetRequiredService<IHttpContextAccessor>()))

                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}