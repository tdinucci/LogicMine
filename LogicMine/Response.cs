using System;

namespace LogicMine
{
    public class Response : IResponse
    {
        public Guid RequestId { get; set; }
        public DateTime Date { get; }
        public string Error { get; set; }

        protected Response()
        {
            Date = DateTime.Now;
        }

        public Response(Guid requestId, string error = null) : this()
        {
            RequestId = requestId;
            Error = error;
        }
    }
}