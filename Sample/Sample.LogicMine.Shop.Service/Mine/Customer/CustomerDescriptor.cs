using LogicMine.DataObject.Ado.Sqlite;

namespace Sample.LogicMine.Shop.Service.Mine.Customer
{
    public class CustomerDescriptor : SqliteMappedObjectDescriptor<Customer, int>
    {
        public CustomerDescriptor() : base("Customer", "Id", nameof(Customer.Id))
        {
        }
    }
}