using System.Threading.Tasks;
using LogicMine;

namespace InterStationComms.Mine
{
    public class HelloTerminal : Terminal<HelloRequest, HelloResponse>
    {
        public override Task AddResponseAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            basket.Response = new HelloResponse(basket.Request)
            {
                Greeting = $"Good {basket.Request.Period.ToString().ToLower()} {basket.Request.Name}"
            };
            return Task.CompletedTask;
        }
    }
}