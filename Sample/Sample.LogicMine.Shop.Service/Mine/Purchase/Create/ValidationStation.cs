using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase.Create
{
    public class ValidationStation : Station<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>> basket)
        {
            var purchase = basket.Request?.Object;
            
            if(purchase == null)
                throw new ValidationException("No purchase provided");
            
            if (purchase.Quantity <= 0 || purchase.Quantity > 100)
                throw new ValidationException("Quantity must be between 1 and 100");

            purchase.CreatedAt = DateTime.Now;
            
            return Task.CompletedTask;
        }
    }
}