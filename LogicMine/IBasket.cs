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

namespace LogicMine
{
  /// <summary>
  /// <para>See <see cref="IBasket"/></para>
  /// An IBasket with ascent and descent payloads of particular types
  /// </summary>
  /// <typeparam name="TDescentPayload">The type of descent (i.e. request) payload</typeparam>
  /// <typeparam name="TAscentPayload">They type of ascent (i.e. response) payload</typeparam>
  public interface IBasket<TDescentPayload, TAscentPayload> : IBasket<TDescentPayload>
  {
    /// <summary>
    /// Typically an object which represents a response
    /// </summary>
    new TAscentPayload AscentPayload { get; set; }
  }

  /// <summary>
  /// <para>See <see cref="IBasket"/></para>
  /// An IBasket with a descent payload of a particular type
  /// </summary>
  /// <typeparam name="TDescentPayload">The type of descent (i.e. request) payload</typeparam>
  public interface IBasket<TDescentPayload> : IBasket
  {
    /// <summary>
    /// Typically an object which represents a request
    /// </summary>
    new TDescentPayload DescentPayload { get; set; }
  }

  /// <summary>
  /// <para>
  /// An IBasket travels along a mine shaft (<see cref="IShaft{TBasket}"/>) and contains all data relevant to a requested operation and the response.
  /// </para>
  /// <para>
  /// A non-trivial shaft will consist of multiple stations (<see cref="IStation{TBasket}"/>) and a terminal (<see cref="ITerminal{TBasket}"/>) 
  /// and each of these waypoints may inspect the contents of the basket and modify them. 
  /// </para>
  /// </summary>
  public interface IBasket
  {
    /// <summary>
    /// The type which the basket deals with.  This does not neccessarily have to marry to the types 
    /// which DescentPayload and AscentPayload contain, it is simply an indication that the basket deals 
    /// with requests related to this type.
    /// </summary>
    Type DataType { get; }

    /// <summary>
    /// Typically an object which represents a request
    /// </summary>
    object DescentPayload { get; }

    /// <summary>
    /// Typically an object which represents a response
    /// </summary>
    object AscentPayload { get; }

    /// <summary>
    /// A note within the basket that one waypoint may leave for others.  These notes can be inspected by 
    /// other waypoints and the shaft itself, so for example, if an <see cref="IInteruptNote"/> is added the the 
    /// basket then the <see cref="IShaft{TBasket}"/> will most likely decide there is no point in continuing 
    /// to pass the basket along the shaft normally
    /// </summary>
    INote Note { get; }

    /// <summary>
    /// Will return true if there was an error while processing the basket
    /// </summary>
    bool IsError { get; }

    /// <summary>
    /// If while a waypoint is processing one basket it creates another one and sends it down a shaft this 
    /// new basket will have it's Parent property set to the original basket.  The original basket will 
    /// have the new basket added to it's Children collection.
    /// </summary>
    IBasket Parent { get; }

    /// <summary>
    /// <see cref="Parent"/> - this contains information about where is a shaft the parent basket was at the 
    /// time the new basket was created
    /// </summary>
    IVisit ParentVisit { get; }

    /// <summary>
    /// The time the basket started it's journey at
    /// </summary>
    DateTime StartedAt { get; }
    
    /// <summary>
    /// The amount of time the basket has travelled for
    /// </summary>
    TimeSpan JourneyDuration { get; set; }

    /// <summary>
    /// Miscellaneous information which may be added to a basket on it's journey which allows stations 
    /// to communicate which allows for general communication between waypoints during a baskets journey.
    /// <para>
    /// This should be used as sparingly as possible because it does come with the risk that waypoints become coupled, 
    /// i.e. waypoint X depends on waypoint Y adding trinket Z and if waypoint Y changes it could break waypoint X.
    /// </para>
    /// </summary>
    IReadOnlyDictionary<string, object> Trinkets { get; }
    
    /// <summary>
    /// A description of each waypoint the basket passed through
    /// </summary>
    IReadOnlyCollection<IVisit> Visits { get; }

    /// <summary>
    /// The child baskets that were created during this baskets journey
    /// </summary>
    IReadOnlyCollection<IBasket> Children { get; }

    /// <summary>
    /// A waypoint may change the note within the basket and the shaft and other waypoints may act upon it.
    /// </summary>
    /// <param name="note"></param>
    void ReplaceNote(INote note);

    /// <summary>
    /// Add a child basket
    /// </summary>
    /// <param name="child"></param>
    void AddChild(IBasket child);

    /// <summary>
    /// Add to the Trinkets collection
    /// </summary>
    /// <param name="name">The trinkets name</param>
    /// <param name="value">The trinkets value</param>
    void AddTrinket(string name, object value);

    /// <summary>
    /// Add a description of a visit to a waypoint
    /// </summary>
    /// <param name="visit"></param>
    void AddVisit(IVisit visit);
  }
}
