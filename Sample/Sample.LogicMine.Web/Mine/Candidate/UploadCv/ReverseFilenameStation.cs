using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.Candidate.UploadCv
{
    public class ReverseFilenameStation : Station<UploadCvRequest, UploadCvResponse>
    {
        public override Task DescendToAsync(IBasket basket, IBasketPayload<UploadCvRequest, UploadCvResponse> payload)
        {
            if (payload?.Request?.Filename != null)
                payload.Request.Filename += "[down-station]";

            return Task.CompletedTask;
        }

        public override Task AscendFromAsync(IBasket basket, IBasketPayload<UploadCvRequest, UploadCvResponse> payload)
        {
            if (payload?.Response?.Filename != null)
                payload.Response.Filename += "[up-station]";

            return Task.CompletedTask;
        }
    }
}