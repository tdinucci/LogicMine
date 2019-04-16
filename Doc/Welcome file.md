---


---

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
<p>All types of request come into a LogicMine service through a single Web API controller.  The LogicMine framework covers everything that’s needed however you need to provide the access point.  Create a new class called <em>MyRequestController</em> and add the following:</p>
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

