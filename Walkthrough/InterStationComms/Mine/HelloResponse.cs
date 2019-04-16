using LogicMine;

namespace InterStationComms.Mine
{
    public class HelloResponse : Response<HelloRequest>
    {
        public string Greeting { get; set; }

        public HelloResponse(HelloRequest request) : base(request)
        {
        }
    }
}