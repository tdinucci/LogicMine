using System;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase
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

        // these fields are used within the shaft only and so are internal
        internal Customer.Customer Customer { get; set; }
        internal Product.Product Product { get; set; }
    }
}