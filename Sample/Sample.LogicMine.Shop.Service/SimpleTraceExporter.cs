using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LogicMine;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Shop.Service
{
    /// <summary>
    /// This is a very basic implementation of ITraceExporter which simply writes trace information to a file on disk.
    /// A more useful implementation would export traces to a service like Application Insights, Datadog, etc.
    ///
    /// The Export() method is called whenever a basket emerges from a shaft and the basket contains a full trace of
    /// the journey it took.
    /// </summary>
    public class SimpleTraceExporter : ITraceExporter, IDisposable
    {
        private readonly StreamWriter _file;

        public SimpleTraceExporter(string traceFilePath)
        {
            if (string.IsNullOrWhiteSpace(traceFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(traceFilePath));

            try
            {
                _file = new StreamWriter(File.Open(traceFilePath, FileMode.Create)) {AutoFlush = true};
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialise trace file: {ex.Message}", ex);
            }
        }

        // This is called whenever a basket emerges from a mine.  You have an opportunity to extract whatever information 
        // is of interest and to send it somewhere.  Here we're just serialising the basket to JSON and lightly tidying 
        // the data.
        //
        // Even if an error occurs when a basket is within a shaft this method will be called and the basket will contain
        // details of the error.
        public void Export(IBasket basket)
        {
            Task.Run(() =>
            {
                var jObject = JObject.FromObject(basket);

                if (jObject.ContainsKey("CurrentVisit"))
                    jObject.Remove("CurrentVisit");

                if (jObject.ContainsKey("IsFlagForRetrieval"))
                    jObject.Remove("IsFlagForRetrieval");

                if (jObject.TryGetValue("Request", out var request))
                {
                    request.First.AddBeforeSelf(new JProperty("Type", basket.Request.GetType().Name));
                }

                _file.WriteLine(
                    $">>>>>>>>>> [{DateTime.Now}]{Environment.NewLine}{jObject}{Environment.NewLine}<<<<<<<<<<{Environment.NewLine}");
            });
        }

        // This is called if an exception occurs outside of a mine.  This can only really happen if an error occurs
        // during service setup or if a request passed to a controller can't be parsed.
        public void ExportError(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("!!! Start Exception !!!");

            var level = 1;
            while (exception != null)
            {
                sb.AppendLine($"=== Level {level++} ===");
                sb.AppendLine(exception.Message);

                exception = exception.InnerException;
            }

            sb.AppendLine("!!! End Exception !!!");

            ExportError(sb.ToString());
        }

        public void ExportError(string error)
        {
            _file.WriteLine($"[{DateTime.Now}] ERROR: {error}");
        }

        public void Dispose()
        {
            _file?.Dispose();
        }
    }
}