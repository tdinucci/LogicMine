using System;
using System.Threading.Tasks;
using LogicMine;

namespace CrossShaft.Mine.GetTime
{
    public class GetTimeTerminal : Terminal<GetTimeRequest, GetTimeResponse>
    {
        public override Task AddResponseAsync(IBasket<GetTimeRequest, GetTimeResponse> basket)
        {
            basket.Response = new GetTimeResponse(basket.Request) {Time = DateTime.Now.ToShortTimeString()};
            return Task.CompletedTask;
        }
    }
}