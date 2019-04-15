using System;
using LogicMine;

namespace Test.Common.LogicMine.Mine.GetTime
{
    public class GetTimeResponse : Response<GetTimeRequest>
    {
        public DateTime? Time { get; }

        public GetTimeResponse(GetTimeRequest request) : base(request)
        {
        }

        public GetTimeResponse(GetTimeRequest request, DateTime time) : this(request)
        {
            Time = time;
        }
    }
    
    public class GetDisposableTimeResponse : Response<GetDisposableTimeRequest>
    {
        public DateTime? Time { get; }

        public GetDisposableTimeResponse(GetDisposableTimeRequest request) : base(request)
        {
        }

        public GetDisposableTimeResponse(GetDisposableTimeRequest request, DateTime time) : this(request)
        {
            Time = time;
        }
    }
}