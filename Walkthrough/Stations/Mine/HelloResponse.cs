using LogicMine;

namespace Stations.Mine
{
    public class HelloResponse : Response<HelloRequest>
    {
        public string Greeting { get; set; }

        public HelloResponse(HelloRequest request) : base(request)
        {
        }
    }
}