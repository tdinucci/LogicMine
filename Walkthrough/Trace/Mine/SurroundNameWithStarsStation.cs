using System.Threading.Tasks;
using LogicMine;

namespace Trace.Mine
{
    public class SurroundNameWithStarsStation : Station<HelloRequest, HelloResponse>
    {
        public override Task DescendToAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            basket.Request.Name = "*" + basket.Request.Name + "*";
            return Task.CompletedTask;
        }
    }
}