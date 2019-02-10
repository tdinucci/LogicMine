using LogicMine;

namespace Sample.LogicMine.Web.Mine.Candidate.UploadCv
{
    public class UploadCvResponse : Response
    {
        public string Filename { get; set; }

        public UploadCvResponse(IRequest request) : base(request)
        {
        }
    }
}