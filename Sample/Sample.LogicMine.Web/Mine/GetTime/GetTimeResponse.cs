using System;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.GetTime
{
    public class GetTimeResponse : Response
    {
        public string Time { get; }

        public GetTimeResponse(IRequest request) : base(request)
        {
        }

        public GetTimeResponse(IRequest request, DateTime time) : base(request)
        {
            Time = time.ToLongTimeString();
        }
    }
}