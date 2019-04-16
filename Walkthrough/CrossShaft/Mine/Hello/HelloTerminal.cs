using System.Threading.Tasks;
using CrossShaft.Mine.GetTime;
using LogicMine;

namespace CrossShaft.Mine.Hello
{
    public class HelloTerminal : Terminal<HelloRequest, HelloResponse>
    {
        public override async Task AddResponseAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            var timeResponse = await Within.Within
                .SendAsync<GetTimeRequest, GetTimeResponse>(basket, new GetTimeRequest()).ConfigureAwait(false);

            basket.Response = new HelloResponse(basket.Request)
            {
                Greeting = $"Hello {basket.Request.Name} the time is {timeResponse.Time}"
            };
        }
    }
}