using LogicMine;

namespace Test.Common.LogicMine.Mine.GetDeconstructedDate
{
    public class GetDeconstructedDateRespone : Response<GetDeconstructedDateRequest>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        public GetDeconstructedDateRespone(GetDeconstructedDateRequest request) : base(request)
        {
        }
    }
}