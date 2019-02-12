using System;

namespace LogicMine
{
    /// <summary>
    /// Represents a response to a caller based on some request, e.g. the object that the caller requested
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// The id of the request that lead to the response - <see cref="IRequest"/>.
        /// This may be Guid.Empty if an error occurred before a request was fully constructed
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// The date the response was generated
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// Any error that occurred while attempting to produce the response, normally this will be null
        /// </summary>
        string Error { get; set; }
    }
}