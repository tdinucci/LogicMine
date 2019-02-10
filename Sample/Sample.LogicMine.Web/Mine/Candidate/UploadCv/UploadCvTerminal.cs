using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.Candidate.UploadCv
{
    public class UploadCvTerminal : Terminal<UploadCvRequest, UploadCvResponse>
    {
        public override Task AddResponseAsync(IBasket<UploadCvRequest, UploadCvResponse> basket)
        {
            basket.Payload.Response = new UploadCvResponse(basket.Payload.Request)
            {
                Filename = basket.Payload.Request?.Filename?.ToUpper()
            };
            return Task.CompletedTask;
        }
    }
}