using System.Threading.Tasks;
using LogicMine;

namespace HelloWorld.Mine
{
    public class HelloTerminal : Terminal<HelloRequest, HelloResponse>
    {
        public override Task AddResponseAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            basket.Response = new HelloResponse(basket.Request) {Greeting = "Hello " + basket.Request.Name};
            return Task.CompletedTask;
        }
    }
}