using System;
using System.Diagnostics;
using System.Net.Http;

namespace Sample.LogicMine.WebApi.Client
{
  class Program
  {
    static void Main()
    {
      try
      {
        Console.WriteLine("Hit return to start");
        Console.ReadLine();
        Console.WriteLine("Starting...");

        var sw = Stopwatch.StartNew();

        using (var client = new HttpClient())
        {
          // this is URL Sample.LogicMine.WebApi is assumed to be listening at
          client.BaseAddress = new Uri("http://localhost:2148");
          new Genesis("kermit", client).LetThereBeLightAsync().Wait();
        }

        sw.Stop();
        Console.WriteLine($">>> {sw.Elapsed.TotalSeconds}s");
      }
      catch (Exception ex)
      {
        do
        {
          Console.WriteLine("*** " + ex.Message);
          ex = ex.InnerException;
        } while (ex != null);
      }

      Console.WriteLine("Hit return to exit");
      Console.ReadLine();
    }
  }
}
