using System;

namespace LogicMine
{
    public class Response : IResponse
    {
        public DateTime Date { get; }
        public string Error { get; set; }

        public Response()
        {
            Date = DateTime.Now;
        }

        public Response(string error = null) : this()
        {
            Error = error;
        }
    }
}