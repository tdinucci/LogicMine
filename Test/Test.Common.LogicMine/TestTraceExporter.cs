using System;
using System.Text;
using LogicMine;

namespace Test.Common.LogicMine
{
    public class TestTraceExporter : ITraceExporter
    {
        public string Trace { get; private set; }
        public string Error { get; private set; }

        public void Export(IBasket basket)
        {
            var sb = new StringBuilder();
            foreach (var visit in basket.Visits)
            {
                sb.AppendLine($"{visit.Description} {visit.Direction}");
                foreach (var logMessage in visit.LogMessages)
                    sb.AppendLine($"\t{logMessage}");
            }

            Trace = sb.ToString();
        }

        public void ExportError(Exception exception)
        {
            Error = exception.Message;
        }

        public void ExportError(string error)
        {
            Error = error;
        }
    }
}