using LogicMine;

namespace CrossShaft.Mine.GetTime
{
    public class GetTimeResponse : Response<GetTimeRequest>
    {
        public string Time { get; set; }
        
        public GetTimeResponse(GetTimeRequest request) : base(request)
        {
        }
    }
}