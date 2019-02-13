using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Sample.LogicMine.Shop.Service.Mine.Product.Create
{
    public class ValidationStation : Station<CreateObjectRequest<Product>, CreateObjectResponse<Product, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Product>, CreateObjectResponse<Product, int>> basket)
        {
            var issues = new StringBuilder();

            var product = basket.Request?.Object;
            if (product == null)
                issues.AppendLine("No product provided");
            else
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                    issues.AppendLine("Name is required");
                else if (product.Name.Length > 50)
                    issues.AppendLine("Name must 50 characters or fewer");
                
                if (product.Price <= 0)
                    issues.AppendLine("Price must be positive");
                else if (product.Price > 1000)
                    issues.AppendLine("Price must 1000.00 or lower");
                else
                {
                    var priceString = product.Price.ToString(CultureInfo.InvariantCulture);
                    var dotIndex = priceString.IndexOf('.');
                    if (dotIndex > -1 && dotIndex + 3 < priceString.Length)
                        issues.AppendLine("Price can be to a maximum precision of 2 decimal places");
                }
            }

            if (issues.Length > 0)
                throw new ValidationException($"Validation failed: {issues}");

            return Task.CompletedTask;
        }
    }
}