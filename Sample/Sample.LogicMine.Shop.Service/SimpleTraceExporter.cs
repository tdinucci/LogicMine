using System;
using System.IO;
using System.Threading.Tasks;
using LogicMine;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Shop.Service
{
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

        public void ExportError(Exception exception)
        {
            ExportError(exception.Message);
        }

        public void ExportError(string error)
        {
            _file.WriteLine($"[{DateTime.Now}] ERROR: {error}");
        }

        public void Dispose()
        {
            _file?.Flush();
            _file?.Dispose();
        }
    }
}