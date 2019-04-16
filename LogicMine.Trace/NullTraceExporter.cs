using System;

namespace LogicMine.Trace
{
    public class NullTraceExporter : ITraceExporter
    {
        public void ExportError(Exception exception)
        {
        }

        public void ExportError(string error)
        {
        }

        public void Export(IBasket basket)
        {
        }
    }
}