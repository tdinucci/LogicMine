using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.SalesSummary
{
    public class SalesSummaryResponse : Response
    {
        public int NumberOfSales { get; set; }
        public decimal Sales { get; set; }
        public decimal Discounts { get; set; }
        public decimal SubTotal => Sales - Discounts;
        public decimal AverageOrderValue => SubTotal == 0 ? 0 : Math.Round(SubTotal / NumberOfSales, 2);

        public SalesSummaryResponse(IRequest request) : base(request)
        {
        }
    }
}