using System.Threading.Tasks;
using LogicMine;

namespace Trace.Mine
{
    public class MakeNameUppercaseStation : Station<HelloRequest, HelloResponse>
    {
        public override Task DescendToAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            basket.Request.Name = basket.Request.Name.ToUpper();
            return Task.CompletedTask;
        }
    }
}