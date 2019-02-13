using System;

namespace Sample.LogicMine.Shop.Client
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitDiscount { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public override string ToString()
        {
            return $"[{Id}] - {Quantity} * £{UnitPrice:F2} with unit discount of £{UnitDiscount} " +
                   $"- Created At {CreatedAt}";
        }
    }
}