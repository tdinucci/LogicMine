---


---

<p>This walkthrough will take you through the process of building a very simple service that will allow us to ask a it to say “hello” to you personally.  How exciting :D</p>
<p>You may feel that you’re doing a lot of work for such a feeble payout and you’d be correct, there are much easier ways to get a computer to say hello to you.  However, LogicMine has been designed with complex applications in mind that are liable to be actively worked on over years and probably by teams of developers.  In such cases putting in a little extra effort at the start can come with massive rewards.</p>
<p>Now, onwards…</p>
<h4 id="create-a-new-web-api-project">1 . Create a new Web API project</h4>
<pre><code>dotnet new webapi
</code></pre>
<h4 id="add-a-reference-to-logicmine.web">2. Add a reference to <a href="https://www.nuget.org/packages/LogicMine.Web/">LogicMine.Web</a></h4>
<pre><code>dotnet add package LogicMine.Web
</code></pre>
<h4 id="implement-ierrorexporter">3. Implement IErrorExporter</h4>
<p>LogicMine will gently guide you down the path of logging what goes on in your system.  At the very least it expects that you define a mechanism for logging errors.  For now we just want to log errors to the console.</p>
<p>Create a class called <em>MyErrorExporter</em> and add the following:</p>
<pre><code>using System;
using LogicMine;

namespace HelloWorld
{
    public class MyErrorExporter : IErrorExporter
    {
        public void ExportError(Exception exception)
        {
            ExportError(exception.Message);
        }

        public void ExportError(string error)
        {
            Console.Error.WriteLine("Error: " + error);
        }
    }
}
</code></pre>
<h4 id="create-the-application-request-controller">4. Create the application request Controller</h4>
<p>All types of request come into a LogicMine service through a single Web API controller.  The LogicMine framework covers everything that’s needed however you need to provide an access point.  Create a new class called <em>MyRequestController</em> and add the following:</p>
<pre><code>using LogicMine;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HelloWorld.Controllers
{
    [Route("api")]
    public class MyRequestController : JsonRequestController
    {
        public MyRequestController(IRequestRouter&lt;JObject&gt; requestRouter, IErrorExporter errorExporter) :
        base(requestRouter, errorExporter)
        {
        }
    }
}
</code></pre>
<h4 id="define-a-request">5. Define a request</h4>
<p>LogicMine services are message based and we’ll define our request as:</p>
<pre><code>using LogicMine;

namespace HelloWorld.Mine
{
    public class HelloRequest : Request
    {
        public string Name { get; set; }
    }
}
</code></pre>
<h4 id="define-a-response">6. Define a response</h4>
<p>The response to a <em>HelloRequest</em> will look like:</p>
<pre><code>using LogicMine;

namespace HelloWorld.Mine
{
    public class HelloResponse : Response&lt;HelloRequest&gt;
    {
        public string Greeting { get; set; }

        public HelloResponse(HelloRequest request) : base(request)
        {
        }
    }
}
</code></pre>
<h4 id="define-a-terminal">7. Define a terminal</h4>
<p>Each request travels down a processing pipeline (in LogicMine parlance this is referred to as a <em>Shaft</em>).  At the bottom of every shaft is a terminal and this is where a response to a request is generated.</p>
<p>You will notice that the request and response travel through the shaft in an <em>IBasket</em>.  We’ll get into more details on baskets in a later walk-through. For now it is enough to know that this is where our request is held and where we’ll put the response.</p>
<p>In a production application it is highly likely that terminals will perform operations that benefit from asynchronous processing, e.g. querying a database, calling a web service, etc.  For this reason all touchpoints within LogicMine allow for asynchronous processing.  With our simple example though there is no need for this so we just return a completed task to signify that the work within the terminal has finished.</p>
<pre><code>using System.Threading.Tasks;
using LogicMine;

namespace HelloWorld.Mine
{
    public class HelloTerminal : Terminal&lt;HelloRequest, HelloResponse&gt;
    {
        public override Task AddResponseAsync(IBasket&lt;HelloRequest, HelloResponse&gt; basket)
        {
            basket.Response = new HelloResponse(basket.Request) {Greeting = "Hello " + basket.Request.Name};
            return Task.CompletedTask;
        }
    }
}
</code></pre>
<h4 id="define-a-shaft-registrar">8. Define a shaft registrar</h4>
<p>So far we’ve defined our request and response types and also the way in which a response will be generated from a request.  The next step is to stitch this stuff together so that when a <em>HelloRequest</em> comes into our service it can pass down an appropriate shaft so that it hits our <em>HelloTerminal</em>.</p>
<p>For this we’ll create a <em>ShaftRegistrar</em> which allows us to define the structure of our shaft and inject it into our applications <em>Mine</em> (a mine is the complete set of shafts for an application).</p>
<pre><code>using LogicMine;

namespace HelloWorld.Mine
{
    public class HelloShaftRegistrar : ShaftRegistrar
    {
        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new Shaft&lt;HelloRequest, HelloResponse&gt;(new HelloTerminal()));
        }
    }
}
</code></pre>
<h4 id="modify-startup.cs">9. Modify Startup.cs</h4>
<p>If you created the service with <em>dotnet new webapi</em> then you should already have a Startup.cs file.  This needs to be modified so that everything that’s needed to run our mine is set up.</p>
<p>There are many options here however we’ll keep things simple for now.</p>
<p>LogicMine makes heavy use of the dependency-injection pattern and the standard .Net DI container is perfectly fine for our current needs.  We will tell the DI container about the <em>IErrorExporter</em> we defined earlier and also an <em>IRequestRouter</em>.</p>
<p>Request routers intercept requests and forward them to appropriate shafts.  You may have noticed that the <em>MyRequestController</em> class accepts such a construct.  Quite simply the code below is saying that an <em>IntelligentJsonRequestRouter</em> will get passed into this controller and this will then be used to route our requests.</p>
<p>The use of the word “Intelligent” is perhaps a little misleading because there isn’t really any magic going on here.  What the <em>IntelligentJsonRequestRouter</em> does do though that more trivial implementations of <em>IRequestRouter</em> may not is that it discovers all of the shafts registrars within the assembly given to it and automatically executes them.  This means that we can continue building upon our project without having to think about request routing again.</p>
<pre><code>using LogicMine;
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
                .AddSingleton&lt;IRequestRouter&lt;JObject&gt;&gt;(requestRouter)
                .AddSingleton&lt;IErrorExporter, MyErrorExporter&gt;()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
</code></pre>

