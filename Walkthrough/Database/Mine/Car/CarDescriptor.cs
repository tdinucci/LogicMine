using LogicMine.DataObject.Ado.Sqlite;

namespace Database.Mine.Car
{
    public class CarDescriptor : SqliteMappedObjectDescriptor<Car, int>
    {
        public CarDescriptor() : base("car", "id", nameof(Car.Id))
        {
        }
    }
}