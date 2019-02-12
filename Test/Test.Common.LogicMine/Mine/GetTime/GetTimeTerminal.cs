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
}