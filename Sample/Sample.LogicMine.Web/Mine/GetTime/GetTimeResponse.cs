using System;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.GetTime
{
    public class GetTimeResponse : Response
    {
        public string Time { get; }

        public GetTimeResponse()
        {
        }

        public GetTimeResponse(Guid requestId, DateTime time) : base(requestId)
        {
            Time = time.ToLongTimeString();
        }
    }
}