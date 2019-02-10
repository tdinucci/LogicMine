using System;

namespace LogicMine
{
    public class Response : IResponse
    {
        public Guid RequestId { get; set; }
        public DateTime Date { get; }
        public string Error { get; set; }

        public Response(IRequest request)
        {
            RequestId = request?.Id ?? Guid.Empty;
            Date = DateTime.Now;
        }

        public Response(IRequest request, string error) : this(request)
        {
            Error = error;
        }
    }
}