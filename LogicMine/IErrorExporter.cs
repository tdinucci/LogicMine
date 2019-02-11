using System;

namespace LogicMine
{
    /// <summary>
    /// A type that can export errors somewhere, e.g. to a log
    /// </summary>
    public interface IErrorExporter
    {
        /// <summary>
        /// Exports a general exception
        /// </summary>
        /// <param name="exception">The exception</param>
        void ExportError(Exception exception);

        /// <summary>
        /// Exports a general error 
        /// </summary>
        /// <param name="error">The error message</param>
        void ExportError(string error);
    }
}