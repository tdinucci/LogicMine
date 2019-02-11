using System;
using System.Collections.Generic;

namespace LogicMine
{
    /// <inheritdoc />
    public class Request : IRequest
    {
        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public IDictionary<string, object> Options { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Constructs a new Request
        /// </summary>
        protected Request()
        {
        }
    }
}