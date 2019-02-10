using System;
using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.GetTime
{
    public class GetTimeTerminal : Terminal<GetTimeRequest, GetTimeResponse>
    {
        public override Task AddResponseAsync(IBasket<GetTimeRequest, GetTimeResponse> basket)
        {
            basket.Payload.Response = new GetTimeResponse(basket.Payload.Request, DateTime.Now);
            return Task.CompletedTask;
        }
    }
}