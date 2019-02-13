using System;
using System.Text;
using LogicMine;

namespace Sample.LogicMine.Shop.Service
{
    public class SampleTraceExporter : ITraceExporter
    {
        public string Trace { get; private set; }
        public string Error { get; private set; }

        public void Export(IBasket basket)
        {
            var sb = new StringBuilder();
            foreach (var visit in basket.Visits)
                sb.AppendLine($"{visit.Description} {visit.Direction}");

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