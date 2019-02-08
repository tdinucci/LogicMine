using System;

namespace LogicMine
{
    /// <summary>
    /// A type that can extract the trace information from an <see cref="IBasket"/> and export it somewhere
    /// </summary>
    public interface ITraceExporter
    {
        /// <summary>
        /// Exports the baskets trace information
        /// </summary>
        /// <param name="basket">The basket to export trace information for</param>
        void Export(IBasket basket);

        /// <summary>
        /// Exports a general exception to the trace log
        /// </summary>
        /// <param name="exception">The exception</param>
        void ExportError(Exception exception);

        /// <summary>
        /// Exports a general error to the trace log
        /// </summary>
        /// <param name="error">The error message</param>
        void ExportError(string error);
    }
}