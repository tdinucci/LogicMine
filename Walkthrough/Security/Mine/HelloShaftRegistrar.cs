using LogicMine;

namespace Security.Mine
{
    public class HelloShaftRegistrar : ShaftRegistrar
    {
        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new MyShaft<HelloRequest, HelloResponse>(new HelloTerminal()));
        }
    }
}