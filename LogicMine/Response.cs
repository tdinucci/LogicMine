using System;

namespace LogicMine
{
    /// <inheritdoc />
    public class Response : IResponse
    {
        /// <inheritdoc />
        public Guid RequestId { get; set; }

        /// <inheritdoc />
        public DateTime Date { get; }

        /// <inheritdoc />
        public string Error { get; set; }

        /// <summary>
        /// Constructs a new Response
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        public Response(IRequest request)
        {
            RequestId = request?.Id ?? Guid.Empty;
            Date = DateTime.Now;
        }

        /// <summary>
        /// Constructs a new error Response
        /// </summary>
        /// <param name="request">The request that lead to the response</param>
        /// <param name="error">The error that occurred while attempting to produce the response</param>
        public Response(IRequest request, string error) : this(request)
        {
            Error = error;
        }
    }
}