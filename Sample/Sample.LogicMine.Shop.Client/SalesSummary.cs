using System;

namespace Sample.LogicMine.Shop.Client
{
    public class SalesSummary
    {
        public int NumberOfSales { get; set; }
        public decimal Sales { get; set; }
        public decimal Discounts { get; set; }
        public decimal SubTotal => Sales - Discounts;
        public decimal AverageOrderValue => SubTotal == 0 ? 0 : Math.Round(SubTotal / NumberOfSales, 2);

        public override string ToString()
        {
            return $"Sales Amount: £{Sales:F2}, Discounts: £{Discounts:F2}, " +
                   $"Sub Total: £{SubTotal:F2}, No. of Sales: {NumberOfSales}, Average Sale: £{AverageOrderValue:F2}";
        }
    }
}