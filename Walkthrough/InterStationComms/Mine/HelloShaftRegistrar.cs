using LogicMine;

namespace InterStationComms.Mine
{
    public class HelloShaftRegistrar : ShaftRegistrar
    {
        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new Shaft<HelloRequest, HelloResponse>(new HelloTerminal(),
                new CalculatePeriodOfDayStation()));
        }
    }
}