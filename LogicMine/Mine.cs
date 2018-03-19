/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LogicMine
{
  /// <summary>
  /// <para>
  /// This class is the basis for a mine for a particular data type.  On it's own a mine is of little value, 
  /// it is only through the introduction of shafts that real work can be performed.
  /// </para>
  /// <para>
  /// Once shafts are introduced it will be possible to retreive and manipulate objects.
  /// </para>
  /// </summary>
  /// <typeparam name="T">The type stored within the mine</typeparam>
  public class Mine<T> : IMine<T>
  {
    private readonly List<IStationLayer<T>> _stationLayers = new List<IStationLayer<T>>();

    /// <inheritdoc />
    public Type DataType => typeof(T);

    /// <summary>
    /// An object that can export the trace data held within baskets
    /// </summary>
    protected ITraceExporter TraceExporter { get; }

    /// <summary>
    /// The terminal layer within the mine
    /// </summary>
    protected ITerminalLayer<T> TerminalLayer { get; }

    /// <summary>
    /// The layers of stations within the mine
    /// </summary>
    protected IReadOnlyCollection<IStationLayer<T>> StationLayers =>
      new ReadOnlyCollection<IStationLayer<T>>(_stationLayers);

    /// <summary>
    /// Construct a new Mine
    /// </summary>
    /// <param name="terminalLayer">The mines terminal layer</param>
    protected Mine(ITerminalLayer<T> terminalLayer) : this(terminalLayer, null)
    {
    }

    /// <summary>
    /// Construct a new Mine
    /// </summary>
    /// <param name="terminalLayer">The mines terminal layer</param>
    /// <param name="traceExporter">An object that can export the trace data held within baskets</param>
    protected Mine(ITerminalLayer<T> terminalLayer, ITraceExporter traceExporter)
    {
      TerminalLayer = terminalLayer ?? throw new ArgumentNullException(nameof(terminalLayer));
      TraceExporter = traceExporter;
    }

    /// <inheritdoc />
    public IMine<T> Add(IStationLayer<T> layer)
    {
      _stationLayers.Add(layer);
      return this;
    }

    /// <inheritdoc />
    public IStation<TBasket>[] GetStations<TBasket>() where TBasket : IBasket
    {
      var result = new List<IStation<TBasket>>();
      foreach (var layer in _stationLayers)
      {
        if (layer is IStation<TBasket> castLayer)
          result.Add(castLayer);
      }

      return result.ToArray();
    }
  }
}
