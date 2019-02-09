using System;

namespace LogicMine
{
    public interface IResponse
    {
        Guid RequestId { get; set; }
        DateTime Date { get; }
        string Error { get; set; }
    }
}