using System;
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