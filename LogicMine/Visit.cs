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
  /// Represents a visit to a waypoint within a shaft
  /// </summary>
  public class Visit : IVisit
  {
    private readonly IList<string> _logMessages = new List<string>();
    
    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public VisitDirections Direction { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; set; }

    /// <inheritdoc />
    public IEnumerable<string> LogMessages => new ReadOnlyCollection<string>(_logMessages);

    /// <inheritdoc />
    public Exception Exception { get; set; }

    /// <summary>
    /// Log a message for the visit
    /// </summary>
    /// <param name="message"></param>
    public void Log(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
        _logMessages.Add($"[{DateTime.UtcNow.TimeOfDay}] {message}");
    }

    /// <summary>
    /// Construct a new Visit
    /// </summary>
    /// <param name="description">A description of the visit</param>
    /// <param name="direction">The direction that was being moved within the shaft at the time of the visit</param>
    public Visit(string description, VisitDirections direction)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

      Description = description;
      Direction = direction;
    }

    /// <summary>
    /// Construct a new Visit
    /// </summary>
    /// <param name="description">A description of the visit</param>
    /// <param name="direction">The direction that was being moved within the shaft at the time of the visit</param>
    /// <param name="duration">The duration of the visit</param>
    public Visit(string description, VisitDirections direction, TimeSpan duration)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

      Description = description;
      Direction = direction;
      Duration = duration;
    }
  }
}
