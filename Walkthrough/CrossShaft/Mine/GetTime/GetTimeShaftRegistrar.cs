using LogicMine;

namespace CrossShaft.Mine.GetTime
{
    public class GetTimeShaftRegistrar : ShaftRegistrar
    {
        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new Shaft<GetTimeRequest, GetTimeResponse>(new GetTimeTerminal()));
        }
    }
}