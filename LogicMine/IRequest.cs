using System.Collections.Generic;

namespace LogicMine
{
    public interface IRequest
    {
        IDictionary<string, object> Options { get; }
    }
}