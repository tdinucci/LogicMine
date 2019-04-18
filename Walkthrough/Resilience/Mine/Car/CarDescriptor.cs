using LogicMine.DataObject.Ado.Sqlite;

namespace Resilience.Mine.Car
{
    public class CarDescriptor : SqliteMappedObjectDescriptor<Car, int>
    {
        public CarDescriptor() : base("car", "id", nameof(Car.Id))
        {
        }
    }
}