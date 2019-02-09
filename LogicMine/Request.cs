using System;
using System.Collections.Generic;

namespace LogicMine
{
    public class Request : IRequest
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IDictionary<string, object> Options { get; } = new Dictionary<string, object>();

        protected Request()
        {
        }
    }
}