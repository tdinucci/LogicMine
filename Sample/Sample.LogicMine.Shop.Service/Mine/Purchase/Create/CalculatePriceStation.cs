using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase.Create
{
    // A sample station that calculates pricing for a purchase before it's committed to the data store
    public class CalculatePriceStation : Station<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>> basket)
        {
            var purchase = basket.Request.Object;
            purchase.UnitPrice = purchase.Product.Price;

            var discountPercentage = purchase.Quantity / 10 * 0.02m;
            purchase.UnitDiscount = Math.Round(purchase.UnitPrice * discountPercentage, 2);

            return Task.CompletedTask;
        }
    }
}