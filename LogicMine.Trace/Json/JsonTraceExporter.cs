using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LogicMine.Trace.Json
{
    /// <inheritdoc />
    public abstract class JsonTraceExporter : ITraceExporter
    {
        private readonly string _serviceName;

        protected JsonTraceExporter(string serviceName)
        {
            _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        }

        /// <summary>
        /// Saves the log message
        /// </summary>
        /// <param name="message"></param>
        protected abstract void ExportLogMessage(string message);

        /// <inheritdoc />
        public void Export(IBasket basket)
        {
            Task.Run(() =>
            {
                var logMessage = new JsonLoggerBasket(_serviceName, basket).Serialise();
                ExportLogMessage(logMessage);
            });
        }

        /// <inheritdoc />
        public void ExportError(Exception exception)
        {
            var message = new StringBuilder();
            var level = 0;
            while (exception != null)
            {
                message.AppendLine($"==== Level {level++} ====");
                message.AppendLine(exception.Message);
                message.AppendLine($"----\n{exception.StackTrace}");

                exception = exception.InnerException;
            }

            ExportError(message.ToString());
        }

        /// <inheritdoc />
        public void ExportError(string error)
        {
            var log = new JObject
            {
                {"status", "Error"},
                {"error", error}
            };

            ExportLogMessage(log.ToString());
        }
    }
}