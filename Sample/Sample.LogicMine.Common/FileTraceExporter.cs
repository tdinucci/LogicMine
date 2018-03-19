using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LogicMine;

namespace Sample.LogicMine.Common
{
  /// <summary>
  /// This is a basic implementation of ITraceExporter which writes trace information related to baskets 
  /// to a text file.  For improved performance it only flushes it's buffer to disk every 2 seconds.
  /// </summary>
  public class FileTraceExporter : ITraceExporter, IDisposable
  {
    private static readonly object OutputLocker = new object();
    private static readonly TimeSpan FlushFrequency = TimeSpan.FromSeconds(2);

    private readonly string _path;
    private readonly StringBuilder _buffer = new StringBuilder();

    private readonly Timer _flushTimer;

    public FileTraceExporter(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

      _path = path;
      _flushTimer = new Timer(s => Flush(), null, FlushFrequency, FlushFrequency);
    }

    public FileTraceExporter Dump(IBasket basket)
    {
      lock (OutputLocker)
      {
        // This will be called each time a shaft completes.  This ITraceExporter implementation is only interested 
        // in dumping root baskets and it will then iterate over each child - your implementation 
        // may do something different
        if (basket.Parent == null)
        {
          _buffer.AppendLine($"*** {DateTime.Now} - {basket.JourneyDuration.TotalSeconds:F4}s [{basket}] ***");
          Dump(basket, 0);
          _buffer.AppendLine();
        }
      }

      return this;
    }

    ITraceExporter ITraceExporter.Export(IBasket basket)
    {
      return Dump(basket);
    }

    private void Dump(IBasket basket, int level)
    {
      lock (OutputLocker)
      {
        var indent = string.Empty;
        if (level > 0)
        {
          for (var i = 0; i < level; i++)
            indent += "\t";
        }

        foreach (var visit in basket.Visits)
        {
          var duration = visit.Duration == null ? "-" : $"{visit.Duration?.TotalSeconds:F4}s";
          _buffer.AppendLine($"{indent}{duration}\t [{visit.Direction}]\t {visit.Description}");

          if (basket.Children?.Count > 0)
          {
            var visitChildren = basket.Children.Where(c => c.ParentVisit == visit);
            foreach (var child in visitChildren)
            {
              _buffer.AppendLine($"\n{indent}>>> {child.JourneyDuration.TotalSeconds:F4}s [{child}] <<<");
              Dump(child, level + 1);
            }
          }
        }
      }
    }

    private void Flush()
    {
      lock (OutputLocker)
      {
        File.AppendAllText(_path, _buffer.ToString());

        _buffer.Clear();
      }
    }

    public void Dispose()
    {
      Flush();
      _flushTimer?.Dispose();
    }
  }
}
