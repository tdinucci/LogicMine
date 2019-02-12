using LogicMine;

namespace Sample.LogicMine.Web.Mine.GetTime
{
    public class GetTimeShaftRegistrar : SampleShaftRegistrar
    {
        public GetTimeShaftRegistrar(ITraceExporter traceExporter) : base(traceExporter)
        {
        }

        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(GetBasicShaft(new GetTimeTerminal()));
        }
    }
}