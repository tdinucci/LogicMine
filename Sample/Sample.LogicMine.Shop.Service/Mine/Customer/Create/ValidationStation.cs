using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Sample.LogicMine.Shop.Service.Mine.Customer.Create
{
    // A sample station that rejects requests (by throwing an exception) if they don't meet the applications requirements.
    public class ValidationStation : Station<CreateObjectRequest<Customer>, CreateObjectResponse<Customer, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Customer>, CreateObjectResponse<Customer, int>> basket)
        {
            var issues = new StringBuilder();

            var customer = basket.Request?.Object;
            if (customer == null)
                issues.AppendLine("No customer provided");
            else
            {
                if (customer.Email == null || customer.Email.IndexOf('@') < 1)
                    issues.AppendLine("Email is invalid");
                else if (customer.Email.Length > 255)
                    issues.AppendLine("Email must be 255 characters or fewer");

                if (string.IsNullOrWhiteSpace(customer.Forename))
                    issues.AppendLine("Forename is required");
                else if (customer.Forename.Length > 50)
                    issues.AppendLine("Forename must 50 characters or fewer");

                if (string.IsNullOrWhiteSpace(customer.Surname))
                    issues.AppendLine("Surname is required");
                else if (customer.Surname.Length > 50)
                    issues.AppendLine("Surname must 50 characters or fewer");
            }

            if (issues.Length > 0)
                throw new ValidationException($"Validation failed: {issues}");

            customer.CreatedAt = DateTime.Now;
            
            return Task.CompletedTask;
        }
    }
}