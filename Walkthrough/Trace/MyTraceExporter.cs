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