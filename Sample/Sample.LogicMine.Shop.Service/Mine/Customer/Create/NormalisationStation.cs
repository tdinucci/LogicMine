using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Sample.LogicMine.Shop.Service.Mine.Customer.Create
{
    public class NormalisationStation : Station<CreateObjectRequest<Customer>, CreateObjectResponse<Customer, int>>
    {
        public override Task DescendToAsync(
            IBasket<CreateObjectRequest<Customer>, CreateObjectResponse<Customer, int>> basket)
        {
            var customer = basket.Request.Object; 
            customer.Surname = customer.Surname.ToUpper();
            customer.Email = customer.Email.ToLower();

            return Task.CompletedTask;
        }
    }
}