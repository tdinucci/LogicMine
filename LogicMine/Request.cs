using System.Collections.Generic;

namespace LogicMine
{
    public class Request : IRequest
    {
        public IDictionary<string, object> Options { get; } = new Dictionary<string, object>();

        protected Request()
        {
        }
    }
}