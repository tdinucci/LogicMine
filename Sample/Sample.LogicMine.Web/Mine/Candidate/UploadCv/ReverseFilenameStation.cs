using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.Candidate.UploadCv
{
    public class ReverseFilenameStation : Station<UploadCvRequest, UploadCvResponse>
    {
        public override Task DescendToAsync(IBasket<UploadCvRequest, UploadCvResponse> basket)
        {
            if (basket?.Request?.Filename != null)
                basket.Request.Filename += "[down-station]";

            return Task.CompletedTask;
        }

        public override Task AscendFromAsync(IBasket<UploadCvRequest, UploadCvResponse> basket)
        {
            if (basket?.Response?.Filename != null)
                basket.Response.Filename += "[up-station]";

            return Task.CompletedTask;
        }
    }
}