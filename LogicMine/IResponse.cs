using System;

namespace LogicMine
{
    /// <summary>
    /// Represents a response to a caller based on some request, e.g. the object that the caller requested.
    /// 
    /// This version of the interface is a marker which ties the response to a specific request type and can be used
    /// elsewhere to ensure a level of compile time safety, e.g. ensure that Stations can't be defined with incompatible
    /// request/response pairs
    /// </summary>
    /// <typeparam name="TRequest">The type of request this response type is paired with</typeparam>
    public interface IResponse<TRequest> : IResponse where TRequest : IRequest
    {
    }

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