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
  /// Represents a visit to a waypoint within a shaft
  /// </summary>
  public interface IVisit
  {
    /// <summary>
    /// A description of the visit
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The direction that was being moved within the shaft at the time of the visit
    /// </summary>
    VisitDirections Direction { get; }

    /// <summary>
    /// The duration of the visit.  If this value is null it means the visit failed or 
    /// or is still in progress
    /// </summary>
    TimeSpan? Duration { get; }

    /// <summary>
    /// Messages that were recorded during the visit
    /// </summary>
    IEnumerable<string> LogMessages { get; }

    /// <summary>
    /// An exception that occurred during the visit
    /// </summary>
    Exception Exception { get; }

    /// <summary>
    /// Log a message related to the visit
    /// </summary>
    /// <param name="message">The message to log</param>
    void Log(string message);
  }
}
