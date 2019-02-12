using System;
using LogicMine;

namespace Test.Common.LogicMine.Mine.GetTime
{
    public class GetTimeResponse : Response
    {
        public DateTime? Time { get; }

        public GetTimeResponse(IRequest request) : base(request)
        {
        }

        public GetTimeResponse(IRequest request, DateTime time) : this(request)
        {
            Time = time;
        }
    }
}