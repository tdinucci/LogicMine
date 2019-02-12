using LogicMine;

namespace Sample.LogicMine.Web.Mine.Candidate.UploadCv
{
    public class UploadCvShaftRegistrar : SampleShaftRegistrar
    {
        public UploadCvShaftRegistrar(ITraceExporter traceExporter) : base(traceExporter)
        {
        }

        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(
                GetBasicShaft(new UploadCvTerminal())
                    .AddToTop(new ReverseFilenameStation()));
        }
    }
}