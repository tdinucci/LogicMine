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
using System.Linq;

namespace LogicMine
{
  /// <summary>
  /// See <see cref="IBasket{TDescentPayload, TAscentPayload}"/>
  /// </summary>
  /// <typeparam name="TDescentPayload">The type of descent (i.e. request) payload</typeparam>
  /// <typeparam name="TAscentPayload">They type of ascent (i.e. response) payload</typeparam>
  public abstract class Basket<TDescentPayload, TAscentPayload> : Basket<TDescentPayload>,
    IBasket<TDescentPayload, TAscentPayload>
  {
    /// <inheritdoc />
    public new TAscentPayload AscentPayload
    {
      get => (TAscentPayload) base.AscentPayload;
      set => base.AscentPayload = value;
    }

    /// <summary>
    /// Construct a new Basket
    /// </summary>
    /// <param name="descentPayload">The payload that will travel down the shaft</param>
    protected Basket(object descentPayload) : base(descentPayload)
    {
    }

    /// <summary>
    /// Construct a new Basket
    /// </summary>
    /// <param name="descentPayload">The payload that will travel down the shaft</param>
    /// <param name="parent">The basket which owns this one</param>
    protected Basket(object descentPayload, IBasket parent) : base(descentPayload, parent)
    {
    }
  }

  /// <summary>
  /// See <see cref="IBasket{TDescentPayload}"/>
  /// </summary>
  /// <typeparam name="TDescentPayload">The type of descent (i.e. request) payload</typeparam>
  public abstract class Basket<TDescentPayload> : Basket, IBasket<TDescentPayload>
  {
    /// <inheritdoc />
    public new TDescentPayload DescentPayload
    {
      get => (TDescentPayload) base.DescentPayload;
      set => base.DescentPayload = value;
    }

    /// <summary>
    /// Construct a new Basket
    /// </summary>
    /// <param name="descentPayload">The payload that will travel down the shaft</param>
    protected Basket(object descentPayload) : base(descentPayload)
    {
    }

    /// <summary>
    /// Construct a new Basket
    /// </summary>
    /// <param name="descentPayload">The payload that will travel down the shaft</param>
    /// <param name="parent">The basket which owns this one</param>
    protected Basket(object descentPayload, IBasket parent) : base(descentPayload, parent)
    {
    }
  }

  /// <summary>
  /// See <see cref="IBasket"/>
  /// </summary>
  public abstract class Basket : IBasket
  {
    private readonly Dictionary<string, object> _trinkets = new Dictionary<string, object>();
    private readonly List<IVisit> _visits = new List<IVisit>();
    private readonly IList<IBasket> _children = new List<IBasket>();

    /// <inheritdoc />
    public abstract Type DataType { get; }

    /// <inheritdoc />
    public object DescentPayload { get; protected set; }

    /// <inheritdoc />
    public object AscentPayload { get; protected set; }

    /// <inheritdoc />
    public INote Note { get; private set; }

    /// <inheritdoc />
    public bool IsError => GetIsError(); // don't cache result as basket contents may change

    /// <inheritdoc />
    public IBasket Parent { get; }

    /// <inheritdoc />
    public IVisit ParentVisit { get; }

    /// <inheritdoc />
    public DateTime StartedAt { get; }
    
    /// <inheritdoc />
    public TimeSpan JourneyDuration { get; set; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> Trinkets => new ReadOnlyDictionary<string, object>(_trinkets);

    /// <inheritdoc />
    public IReadOnlyCollection<IVisit> Visits => new ReadOnlyCollection<IVisit>(_visits);

    /// <inheritdoc />
    public IReadOnlyCollection<IBasket> Children => new ReadOnlyCollection<IBasket>(_children);

    /// <summary>
    /// Construct a new Basket
    /// </summary>
    /// <param name="descentPayload">The payload that will travel down the shaft</param>
    protected Basket(object descentPayload) : this(descentPayload, null)
    {
    }

    /// <summary>
    /// Construct a new Basket
    /// </summary>
    /// <param name="descentPayload">The payload that will travel down the shaft</param>
    /// <param name="parent">The parent of the new basket</param>
    protected Basket(object descentPayload, IBasket parent)
    {
      StartedAt = DateTime.UtcNow;
      Note = new OkNote();
      DescentPayload = descentPayload;

      Parent = parent;
      ParentVisit = parent?.Visits.Last();
      parent?.AddChild(this);
    }

    /// <inheritdoc />
    public void ReplaceNote(INote note)
    {
      Note = note ?? throw new ArgumentNullException(nameof(note));
    }

    /// <inheritdoc />
    public void AddTrinket(string name, object value)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

      if (_trinkets.ContainsKey(name))
        throw new InvalidOperationException($"Basket already contains a trinket called '{name}'");

      _trinkets.Add(name, value);
    }

    /// <inheritdoc />
    public void AddVisit(IVisit visit)
    {
      if (visit == null)
        throw new ArgumentNullException(nameof(visit));

      _visits.Add(visit);
    }

    /// <inheritdoc />
    public void AddChild(IBasket child)
    {
      if (child == null)
        throw new ArgumentNullException(nameof(child));

      _children.Add(child);
    }

    private bool GetIsError()
    {
      if (_visits.FirstOrDefault(v => v.Exception != null) != default(IVisit))
        return true;

      return _children.FirstOrDefault(
               c => c.Visits.FirstOrDefault(v => v.Exception != null) != default(IVisit)) != default(IBasket);
    }
  }
}
