using System.Linq;
using System.Threading.Tasks;
using LogicMine;

namespace Stations.Mine
{
    public class ReverseResponseStation : Station<HelloRequest, HelloResponse>
    {
        public override Task DescendToAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            return Task.CompletedTask;
        }

        public override Task AscendFromAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            basket.Response.Greeting = new string(basket.Response.Greeting.Reverse().ToArray());
            return Task.CompletedTask;
        }
    }
}