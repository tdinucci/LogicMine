## Walkthrough 5 - Tracing
Please make sure you have followed the second walkthrough (Stations) as this one follows on from it.

In a real application you would most likely send log files to some central store or service.  For the purposes of this walkthrough though we are just going to log our traces to file.

#### 1. Open your Stations walkthrough project
*N.B. The code for these walkthroughs is included in the source respository, as a project per walkthrough.  The code within this walkthrough is taken from these projects and the namespaces will be slightly different to yours if you're following along with your own project.*

#### 2. Add a reference to [LogicMine.Trace](https://www.nuget.org/packages/LogicMine.Trace/)

```dotnet add package LogicMine.Trace```

#### 3. Create a trace exporter
We'll base our exporter on the framework provided *JsonTraceExporter* and we'll write our log to a file on disk.  It won't do anything fancy, like buffering messages, etc.

```csharp
using System.IO;
using LogicMine.Trace.Json;

namespace Trace
{
    public class MyTraceExporter : JsonTraceExporter
    {
        private static readonly string LogPath = Path.Combine(Path.GetTempPath(), "logicmine.trc");

        public MyTraceExporter(string serviceName) : base(serviceName)
        {
        }

        protected override void ExportLogMessage(string message)
        {
            File.AppendAllText(LogPath, message);
        }
    }
}
```

#### 4. Modify Startup.cs
Make the router and DI container aware of our trace exporter.  N.B. an ITraceExporter is also an IErrorExporter so we might as well register this too.  If provided an IErrorExporter will be used to log any unexpected errors that occur outside of the mine - which should be very rare and likely only to occur if there are configuration issues which would stop the service from fully starting up anyway.

```csharp
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

```

#### 5. Inject the trace exporter into our shaft
Take your existing *HelloShaftRegistrar* and modify the code so it looks like the following.

In a real application we'd likely want to create our own abstract subclass of *ShaftRegistrar* and then base all concrete registrars on this.  This new subclass would force the concrete classes to provide an ITraceExporter.

```csharp
using LogicMine;

namespace Trace.Mine
{
    public class HelloShaftRegistrar : ShaftRegistrar
    {
        private readonly ITraceExporter _traceExporter;

        public HelloShaftRegistrar(ITraceExporter traceExporter)
        {
            _traceExporter = traceExporter;
        }

        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new Shaft<HelloRequest, HelloResponse>(_traceExporter, new HelloTerminal(),
                new ReverseResponseStation(),
                new MakeNameUppercaseStation(),
                new SurroundNameWithStarsStation()));
        }
    }
}
```

#### 7. Use the service
Just run it the same way you have in the other walkthroughs, we don't expect to see anything different in the output.

#### 8. Inspect the log
The file will have been stored in your temp directory and the content will look something like what's shown below.

Note, if you ever find that you are logging too much information, e.g. the contents of a binary file from an upload request, then you can mark the properties you don't want logged on your request or response types with the *LogicMine.Trace.NoLog* attribute.

```json
{
  "Service": "MyService",
  "RequestId": "cb04f582-e77d-48e6-bd79-006da87f5f27",
  "ParentRequestId": null,
  "RequestType": "HelloRequest",
  "Request": "{\n  \"Name\": \"*JIM*\",\n  \"Id\": \"cb04f582-e77d-48e6-bd79-006da87f5f27\",\n  \"ParentId\": null,\n  \"Options\": {}\n}",
  "Response": "{\n  \"Greeting\": \"*MIJ* olleH\",\n  \"RequestId\": \"cb04f582-e77d-48e6-bd79-006da87f5f27\",\n  \"Date\": \"2019-04-16T19:00:47.065552+01:00\",\n  \"Error\": null\n}",
  "Status": "Info",
  "StartedAt": "2019-04-16T19:00:47.0655372+01:00",
  "Duration": "00:00:00.0000236",
  "Visits": [
    {
      "Description": "[Down] Trace.Mine.ReverseResponseStation",
      "StartedAt": "2019-04-16T19:00:47.0655421+01:00",
      "Duration": "00:00:00.0000022",
      "LogMessages": [],
      "Error": ""
    },
    {
      "Description": "[Down] Trace.Mine.MakeNameUppercaseStation",
      "StartedAt": "2019-04-16T19:00:47.0655458+01:00",
      "Duration": "00:00:00.0000011",
      "LogMessages": [],
      "Error": ""
    },
    {
      "Description": "[Down] Trace.Mine.SurroundNameWithStarsStation",
      "StartedAt": "2019-04-16T19:00:47.0655478+01:00",
      "Duration": "00:00:00.0000009",
      "LogMessages": [],
      "Error": ""
    },
    {
      "Description": "[Down] Trace.Mine.HelloTerminal",
      "StartedAt": "2019-04-16T19:00:47.0655504+01:00",
      "Duration": "00:00:00.0000018",
      "LogMessages": [],
      "Error": ""
    },
    {
      "Description": "[Up] Trace.Mine.SurroundNameWithStarsStation",
      "StartedAt": "2019-04-16T19:00:47.0655534+01:00",
      "Duration": "00:00:00.0000010",
      "LogMessages": [],
      "Error": ""
    },
    {
      "Description": "[Up] Trace.Mine.MakeNameUppercaseStation",
      "StartedAt": "2019-04-16T19:00:47.0655554+01:00",
      "Duration": "00:00:00.0000004",
      "LogMessages": [],
      "Error": ""
    },
    {
      "Description": "[Up] Trace.Mine.ReverseResponseStation",
      "StartedAt": "2019-04-16T19:00:47.0655565+01:00",
      "Duration": "00:00:00.0000026",
      "LogMessages": [],
      "Error": ""
    }
  ]
}
```
