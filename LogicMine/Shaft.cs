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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogicMine
{
  /// <summary>
  /// A shaft is a processing pipeline.  An <see cref="IBasket"/> enters and decends through zero or 
  /// more <see cref="IStation{TBasket}"/>s until it reaches an <see cref="ITerminal{TBasket}"/>.  Each of the stations 
  /// has a chance to inspect and minipulate the baskets contents (and it may return the basket to the surface early if it wants to).  
  /// Once the basket hits the terminal it is filled with a result and sent bask up the shaft again, through the stations it met on the way down.  
  /// Each station again has the chance to inspect and manipulate the baskets contents before it emerges from the shaft.
  /// </summary>
  /// <typeparam name="TBasket">The type of basket the shaft deals with</typeparam>
  public class Shaft<TBasket> : IShaft<TBasket> where TBasket : IBasket
  {
    private readonly List<IStation<TBasket>> _stations = new List<IStation<TBasket>>();

    /// <summary>
    /// Used to export a baskets trace information
    /// </summary>
    protected ITraceExporter TraceExporter { get; }

    /// <summary>
    /// The terminal waypoint within the shaft
    /// </summary>
    protected virtual ITerminal<TBasket> Terminal { get; }
    
    /// <summary>
    /// The stations within the shaft
    /// </summary>
    protected virtual IReadOnlyCollection<IStation<TBasket>> Stations =>
      new ReadOnlyCollection<IStation<TBasket>>(_stations);

    /// <summary>
    /// Construct a new Shaft
    /// </summary>
    /// <param name="terminal">The terminal waypoint</param>
    /// <param name="stations">The stations</param>
    public Shaft(ITerminal<TBasket> terminal, params IStation<TBasket>[] stations) : this(null, terminal, stations)
    {
    }

    /// <summary>
    /// Construct a new Shaft
    /// </summary>
    /// <param name="traceExporter">Used to export a baskets trace information</param>
    /// <param name="terminal">The terminal waypoint</param>
    /// <param name="stations">The stations</param>
    public Shaft(ITraceExporter traceExporter, ITerminal<TBasket> terminal, params IStation<TBasket>[] stations)
    {
      Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
      TraceExporter = traceExporter;

      if (stations != null)
        _stations = stations.ToList();
    }

    /// <summary>
    /// Adds a station to the bottom of the shaft - but above the terminal
    /// </summary>
    /// <param name="station">The station to add</param>
    /// <returns>The current shaft</returns>
    public Shaft<TBasket> Add(IStation<TBasket> station)
    {
      if (station == null)
        throw new ArgumentNullException(nameof(station));

      _stations.Add(station);

      return this;
    }
    
    /// <inheritdoc />
    public async Task<TBasket> SendAsync(TBasket basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      var stopwatch = Stopwatch.StartNew();

      try
      {
        await DescendAsync(basket).ConfigureAwait(false);
        if (basket.Note is IInteruptNote)
          return basket;

        await VisitTerminal(basket).ConfigureAwait(false);
        if (basket.Note is IInteruptNote)
          return basket;

        await AscendAsync(basket).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        throw new ShaftException(
          $"An eror occurred while processing the '{basket.GetType()}'.  See inner exception for details", ex);
      }
      finally
      {
        stopwatch.Stop();
        basket.JourneyDuration = stopwatch.Elapsed;

        TraceExporter?.Export(basket);
      }

      return basket;
    }

    private async Task DescendAsync(TBasket basket)
    {
      var stopwatch = new Stopwatch();
      foreach (var station in _stations)
      {
        stopwatch.Start();

        var visit = new Visit(station.ToString(), VisitDirections.Down);
        basket.AddVisit(visit);

        await station.DescendToAsync(basket).ConfigureAwait(false);

        stopwatch.Stop();
        visit.Duration = stopwatch.Elapsed;
        stopwatch.Reset();

        if (basket.Note is IInteruptNote)
          return;
      }
    }

    private async Task VisitTerminal(TBasket basket)
    {
      var visit = new Visit(Terminal.ToString(), VisitDirections.Down);
      basket.AddVisit(visit);

      var stopwatch = Stopwatch.StartNew();

      await Terminal.AddResultAsync(basket).ConfigureAwait(false);

      stopwatch.Stop();
      visit.Duration = stopwatch.Elapsed;
    }

    private async Task AscendAsync(TBasket basket)
    {
      var stopwatch = new Stopwatch();
      for (var i = _stations.Count - 1; i >= 0; i--)
      {
        stopwatch.Start();

        var station = _stations[i];

        var visit = new Visit(station.ToString(), VisitDirections.Up);
        basket.AddVisit(visit);

        await station.AscendFromAsync(basket).ConfigureAwait(false);

        stopwatch.Stop();
        visit.Duration = stopwatch.Elapsed;
        stopwatch.Reset();

        if (basket.Note is IInteruptNote)
          return;
      }
    }
  }
}
