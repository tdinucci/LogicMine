using System;
using System.Threading.Tasks;
using LogicMine;

namespace Test.Common.LogicMine.Mine.GetDeconstructedDate
{
    public class GetDeconstructedDateTerminal : Terminal<GetDeconstructedDateRequest, GetDeconstructedDateRespone>
    {
        public override Task AddResponseAsync(IBasket<GetDeconstructedDateRequest, GetDeconstructedDateRespone> basket)
        {
            var request = basket.Request;

            var date = DateTime.Now
                .AddYears(request.AddYears)
                .AddMonths(request.AddMonths)
                .AddDays(request.AddDays);

            basket.Response = new GetDeconstructedDateRespone(request)
            {
                Year = date.Year,
                Month = date.Month,
                Day = date.Day,
                Hour = date.Hour,
                Minute = date.Minute,
                Second = date.Second
            };

            return Task.CompletedTask;
        }
    }
}