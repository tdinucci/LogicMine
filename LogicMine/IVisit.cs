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
        VisitDirection Direction { get; }

        /// <summary>
        /// The time the visit started at
        /// </summary>
        DateTime StartedAt { get; }

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