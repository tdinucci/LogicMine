using System;
using System.Threading.Tasks;
using LogicMine;

namespace InterStationComms.Mine
{
    public class CalculatePeriodOfDayStation : Station<HelloRequest, HelloResponse>
    {
        public override Task DescendToAsync(IBasket<HelloRequest, HelloResponse> basket)
        {
            var time = DateTime.Now.TimeOfDay;
            if (time >= TimeSpan.FromHours(4) && time < TimeSpan.FromHours(12))
                basket.Request.Period = HelloRequest.PeriodOfDay.Morning;
            else if (time >= TimeSpan.FromHours(12) && time < TimeSpan.FromHours(18))
                basket.Request.Period = HelloRequest.PeriodOfDay.Afternoon;
            else if (time >= TimeSpan.FromHours(18) && time < TimeSpan.FromHours(23))
                basket.Request.Period = HelloRequest.PeriodOfDay.Evening;
            else
                basket.Request.Period = HelloRequest.PeriodOfDay.Night;

            return Task.CompletedTask;
        }
    }
}