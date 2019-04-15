using System;
using System.Threading.Tasks;
using LogicMine;

namespace Test.Common.LogicMine.Mine.GetTime
{
    public class GetTimeTerminal : Terminal<GetTimeRequest, GetTimeResponse>
    {
        public override Task AddResponseAsync(IBasket<GetTimeRequest, GetTimeResponse> basket)
        {
            basket.Response = new GetTimeResponse(basket.Request, DateTime.Now);
            return Task.CompletedTask;
        }
    }

    public class GetDisposableTimeTerminal : Terminal<GetDisposableTimeRequest, GetDisposableTimeResponse>
    {
        public override Task AddResponseAsync(IBasket<GetDisposableTimeRequest, GetDisposableTimeResponse> basket)
        {
            basket.Response = new GetDisposableTimeResponse(basket.Request, DateTime.Now);
            return Task.CompletedTask;
        }
    }
}