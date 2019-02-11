using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LogicMine
{
    /// <inheritdoc />
    public class Visit : IVisit
    {
        private readonly IList<string> _logMessages = new List<string>();

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public VisitDirection Direction { get; }

        /// <inheritdoc />
        public DateTime StartedAt { get; }

        /// <inheritdoc />
        public TimeSpan? Duration { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> LogMessages => new ReadOnlyCollection<string>(_logMessages);

        /// <inheritdoc />
        public Exception Exception { get; set; }

        /// <summary>
        /// Construct a new Visit
        /// </summary>
        /// <param name="description">A description of the visit</param>
        /// <param name="direction">The direction that was being moved within the shaft at the time of the visit</param>
        public Visit(string description, VisitDirection direction)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

            Description = description;
            Direction = direction;
            StartedAt = DateTime.Now;
        }

        /// <inheritdoc />
        public void Log(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                _logMessages.Add($"[{DateTime.Now.TimeOfDay}] {message}");
        }
    }
}