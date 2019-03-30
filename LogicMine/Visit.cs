using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LogicMine
{
    /// <inheritdoc />
    public class Visit : IVisit
    {
        private readonly IList<string> _logMessages = new List<string>();
        private readonly IList<string> _logWarnings = new List<string>();
        private readonly IList<string> _logErrors = new List<string>();

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
        public IEnumerable<string> LogWarnings => new ReadOnlyCollection<string>(_logWarnings);

        /// <inheritdoc />
        public IEnumerable<string> LogErrors => new ReadOnlyCollection<string>(_logErrors);

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

        /// <inheritdoc />
        public void LogWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning))
                _logWarnings.Add($"[{DateTime.Now.TimeOfDay}] {warning}");
        }

        /// <inheritdoc />
        public void LogError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                _logErrors.Add($"[{DateTime.Now.TimeOfDay}] {error}");
        }

        /// <inheritdoc />
        public void LogError(Exception exception)
        {
            if (exception != null)
                LogError(GetExceptionMessage(exception));
        }

        private string GetExceptionMessage(Exception ex, int aggregateLevel = 0)
        {
            var message = new StringBuilder();
            if (ex is AggregateException agEx)
            {
                foreach (var innerException in agEx.InnerExceptions)
                {
                    message.AppendLine($"---- Aggregate Level {aggregateLevel++}----");
                    message.AppendLine(GetExceptionMessage(innerException, aggregateLevel));
                }
            }
            else
            {
                var level = 0;
                while (ex != null)
                {
                    message.AppendLine($"---- Level {level++} ----");
                    message.AppendLine($"{ex.Message}\n{ex.StackTrace}\n");

                    ex = ex.InnerException;
                }
            }

            return message.ToString();
        }
    }
}