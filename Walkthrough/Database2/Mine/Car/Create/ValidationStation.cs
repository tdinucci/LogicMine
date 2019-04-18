using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Database2.Mine.Car.Create
{
    public class ValidationStation : Station<CreateObjectRequest<Car>, CreateObjectResponse<Car, int>>
    {
        public override Task DescendToAsync(IBasket<CreateObjectRequest<Car>, CreateObjectResponse<Car, int>> basket)
        {
            var errors = new StringBuilder();
            var car = basket.Request.Object;

            if (string.IsNullOrWhiteSpace(car.Make) || car.Make.Length > 50)
                errors.AppendLine("Make is required and must be 50 character or less");

            if (string.IsNullOrWhiteSpace(car.Model) || car.Model.Length > 50)
                errors.AppendLine("Model is required and must be 50 character or less");

            if (car.Year == 0)
                errors.Append("Year is required");

            if (errors.Length > 0)
                throw new ValidationException(errors.ToString());

            return Task.CompletedTask;
        }
    }
}