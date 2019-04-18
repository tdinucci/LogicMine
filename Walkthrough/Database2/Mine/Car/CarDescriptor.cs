using LogicMine.DataObject.Ado.Sqlite;

namespace Database2.Mine.Car
{
    public class CarDescriptor : SqliteMappedObjectDescriptor<Car, int>
    {
        public CarDescriptor() : base("car", "id", nameof(Car.Id))
        {
        }
    }
}