using System;

namespace LogicMine
{
    public interface IResponse
    {
        DateTime Date { get; }
        string Error { get; set; }
    }
}