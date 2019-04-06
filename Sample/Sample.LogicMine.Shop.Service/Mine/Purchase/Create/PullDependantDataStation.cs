using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.GetObject;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase.Create
{
    // This pulls data into the request which can be used by later stations in the shaft, it demonstrates how 
    // one shaft can fork off into another
    public class PullDependantDataStation : Station<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>> basket)
        {
            var request = basket.Request;
            var getCustomerTask = GetObjectAsync<Customer.Customer>(basket, request.Object.CustomerId);
            var getProductTask = GetObjectAsync<Product.Product>(basket, request.Object.ProductId);
            Task.WaitAll(getCustomerTask, getProductTask);

            if (!string.IsNullOrWhiteSpace(getCustomerTask.Result.Error))
                throw new InvalidOperationException($"Failed to determine customer: {getCustomerTask.Result.Error}");

            if (!string.IsNullOrWhiteSpace(getProductTask.Result.Error))
                throw new InvalidOperationException($"Failed to determine product: {getProductTask.Result.Error}");

            basket.Request.Object.Customer = getCustomerTask.Result.Object;
            basket.Request.Object.Product = getProductTask.Result.Object;

            return Task.CompletedTask;
        }

        private async Task<GetObjectResponse<T>> GetObjectAsync<T>(IBasket parent, int id)
        {
            var requestBasket = new GetObjectBasket<T, int>(new GetObjectRequest<T, int>(id));
            await Within.Within.SendAsync(parent, requestBasket);

            return requestBasket.Response;
        }
    }
}