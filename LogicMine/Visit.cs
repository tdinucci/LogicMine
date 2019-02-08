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
        /// <param name="startedAt">The time the visit started at, if null the DateTime.UtcNow will be used</param>
        /// <param name="direction">The direction that was being moved within the shaft at the time of the visit</param>
        public Visit(string description, VisitDirection direction, DateTime? startedAt = null)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

            Description = description;
            Direction = direction;
            StartedAt = startedAt ?? DateTime.UtcNow;
        }

        /// <summary>
        /// Construct a new Visit
        /// </summary>
        /// <param name="description">A description of the visit</param>
        /// <param name="direction">The direction that was being moved within the shaft at the time of the visit</param>
        /// <param name="startedAt">The time the visit started at, if null the DateTime.UtcNow will be used</param>
        /// <param name="duration">The duration of the visit</param>
        public Visit(string description, VisitDirection direction, DateTime? startedAt, TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

            Description = description;
            Direction = direction;
            StartedAt = startedAt ?? DateTime.UtcNow;
            Duration = duration;
        }

        /// <summary>
        /// Log a message for the visit
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                _logMessages.Add($"[{DateTime.UtcNow.TimeOfDay}] {message}");
        }
    }
}