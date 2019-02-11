using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Web.Mine.Candidate.UploadCv
{
    public class UploadCvTerminal : Terminal<UploadCvRequest, UploadCvResponse>
    {
        public override Task AddResponseAsync(IBasket<UploadCvRequest, UploadCvResponse> basket)
        {
            basket.Response = new UploadCvResponse(basket.Request)
            {
                Filename = basket.Request?.Filename?.ToUpper()
            };
            return Task.CompletedTask;
        }
    }
}