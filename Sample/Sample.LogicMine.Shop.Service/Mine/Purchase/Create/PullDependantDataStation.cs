using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.GetObject;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase.Create
{
    public class PullDependantDataStation : Station<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>> basket)
        {
            var request = basket.Request;
            var getCustomerTask = GetObjectAsync<Customer.Customer>(request, request.Object.CustomerId);
            var getProductTask = GetObjectAsync<Product.Product>(request, request.Object.ProductId);
            Task.WaitAll(getCustomerTask, getProductTask);

            if (!string.IsNullOrWhiteSpace(getCustomerTask.Result.Error))
                throw new InvalidOperationException($"Failed to determine customer: {getCustomerTask.Result.Error}");

            if (!string.IsNullOrWhiteSpace(getProductTask.Result.Error))
                throw new InvalidOperationException($"Failed to determine product: {getProductTask.Result.Error}");

            basket.Request.Object.Customer = getCustomerTask.Result.Object;
            basket.Request.Object.Product = getProductTask.Result.Object;

            return Task.CompletedTask;
        }

        private Task<GetObjectResponse<T>> GetObjectAsync<T>(IRequest rootRequest, int id)
        {
            var request = new GetObjectRequest<T, int>(id);

            // relay any security tokens, etc.
            foreach (var option in rootRequest.Options)
                request.Options.Add(option.Key, option.Value);

            return Within.Within.SendAsync<GetObjectRequest<T, int>, GetObjectResponse<T>>(request);
        }
    }
}