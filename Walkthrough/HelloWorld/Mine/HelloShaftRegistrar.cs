using LogicMine;

namespace HelloWorld.Mine
{
    public class HelloShaftRegistrar : ShaftRegistrar
    {
        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new Shaft<HelloRequest, HelloResponse>(new HelloTerminal()));
        }
    }
}