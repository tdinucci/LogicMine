using System.IO;
using LogicMine;
using LogicMine.Api.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sample.LogicMine.Common;

namespace Sample.LogicMine.WebApi
{
  public class Startup
  {
    private static readonly string TraceFilePath = Path.Combine(Path.GetTempPath(), "web-trace.txt");

    private readonly ICache _cache = new InProcessCache();
    private readonly FileTraceExporter _traceExporter;

    public Startup()
    {
      var traceDir = Path.GetDirectoryName(TraceFilePath);

      if (!Directory.Exists(traceDir))
        Directory.CreateDirectory(traceDir);

      _traceExporter = new FileTraceExporter(TraceFilePath);
    }

    public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime)
    {
      applicationLifetime.ApplicationStopping.Register(OnShutdown);

      // create the DB the service will use
      DbUtils.CreateDb();

      app
        .UseMvc();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services
        .AddSingleton(_cache)
        .AddSingleton<ITraceExporter>(_traceExporter)
        .AddTransient<IAuthTokenReader, AuthTokenReader>()
        .AddMvc();
    }

    private void OnShutdown()
    {
      _traceExporter?.Dispose();
      DbUtils.DeleteDb();
    }
  }
}
