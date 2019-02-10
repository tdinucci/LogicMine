using LogicMine;
using LogicMine.DataObject.Salesforce;
using LogicMine.Routing;
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
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";
        private const string SfAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";

        public void ConfigureServices(IServiceCollection services)
        {
            var traceExporter = new DefaultTraceExporter();

            services
                .AddSingleton(
                    new SalesforceCredentials(SfClientId, SfClientSecret, SfUsername, SfPassword, SfAuthEndpoint))
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IErrorExporter>(traceExporter)
                .AddSingleton<ITraceExporter>(traceExporter)
                .AddSingleton(services)
                .AddSingleton<IRequestRouter<JObject>, SampleRequestRouter>()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
