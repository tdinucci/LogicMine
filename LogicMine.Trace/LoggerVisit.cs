using System;
using System.Collections.Generic;

namespace LogicMine.Trace
{
    public class LoggerVisit
    {
        public string Description { get; }
        public DateTime StartedAt { get; }
        public TimeSpan? Duration { get; }
        public IEnumerable<string> LogMessages { get; }
        public string Error { get; }

        public LoggerVisit(IVisit visit)
        {
            if (visit == null)
            {
                Description = "No visit to logger";
                Error = GetExceptionMessage(new Exception("No visit to logger"));
            }
            else
            {
                Description = $"[{visit.Direction}] {visit.Description}";
                StartedAt = visit.StartedAt;
                Duration = visit.Duration;
                LogMessages = visit.LogMessages;
                Error = GetExceptionMessage(visit.Exception);
            }
        }

        private string GetExceptionMessage(Exception ex)
        {
            var level = 0;
            var message = string.Empty;
            while (ex != null)
            {
                message += $"Exception Level {level++}: {ex.Message}.\r\n";
                message += $"===\n{ex.StackTrace}\n===\n";

                ex = ex.InnerException;
            }

            return message;
        }
    }
}