using System;
using System.Diagnostics;
using System.IO;
using Sample.LogicMine.Common;

namespace Sample.LogicMine.Console
{
  /// <summary>
  /// This is a completely useless and contrived program which is intended to offer insight 
  /// into the way you may choose to develop LogicMine applications.
  /// </summary>
  public class Program
  {
    private const string TraceFilePath = @"c:\temp\LogicMine\console-trace.txt";

    public static void Main()
    {
      try
      {
        DbUtils.CreateDb();

        var sw = Stopwatch.StartNew();

        // A trace exporter is optional.  When baskets complete their journey within a shaft they will be passed to an 
        // ITraceExporter (if available) and this will do something like; write to a log, export to Azure App Insights, etc.
        using (var traceExporter = GetTraceExporter())
        {
          // see AppSecurityLayer - "kermit" is our valid user
          new Genesis("kermit", DbUtils.ConnectionString, traceExporter).LetThereBeLightAsync().Wait();
        }

        sw.Stop();
        System.Console.WriteLine($">>> {sw.Elapsed.TotalSeconds}s");
      }
      catch (Exception ex)
      {
        do
        {
          System.Console.WriteLine("*** " + ex.Message);
          ex = ex.InnerException;
        } while (ex != null);
      }
      finally
      {
        DbUtils.DeleteDb();
      }

      System.Console.WriteLine("Hit return to exit...");
      System.Console.ReadLine();
    }

    private static FileTraceExporter GetTraceExporter()
    {
      var traceDir = Path.GetDirectoryName(TraceFilePath);

      if (!Directory.Exists(traceDir))
        Directory.CreateDirectory(traceDir);

      return new FileTraceExporter(TraceFilePath);
    }
  }
}
