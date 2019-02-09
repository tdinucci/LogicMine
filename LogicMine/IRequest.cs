using System;
using System.Collections.Generic;

namespace LogicMine
{
    public interface IRequest
    {
        Guid Id { get; }
        IDictionary<string, object> Options { get; }
    }
}