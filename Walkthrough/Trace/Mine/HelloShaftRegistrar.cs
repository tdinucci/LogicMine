using LogicMine;

namespace Trace.Mine
{
    public class HelloShaftRegistrar : ShaftRegistrar
    {
        private readonly ITraceExporter _traceExporter;

        public HelloShaftRegistrar(ITraceExporter traceExporter)
        {
            _traceExporter = traceExporter;
        }

        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(new Shaft<HelloRequest, HelloResponse>(_traceExporter, new HelloTerminal(),
                new ReverseResponseStation(),
                new MakeNameUppercaseStation(),
                new SurroundNameWithStarsStation()));
        }
    }
}