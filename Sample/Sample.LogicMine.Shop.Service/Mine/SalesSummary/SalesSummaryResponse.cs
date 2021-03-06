using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.SalesSummary
{
    // The response to a sales summary request
    public class SalesSummaryResponse : Response<SalesSummaryRequest>
    {
        public int NumberOfSales { get; set; }
        public decimal Sales { get; set; }
        public decimal Discounts { get; set; }

        public SalesSummaryResponse(SalesSummaryRequest request) : base(request)
        {
        }
    }
}