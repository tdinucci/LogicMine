using LogicMine.DataObject.Ado.Sqlite;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase
{
    public class PurchaseDescriptor : SqliteMappedObjectDescriptor<Purchase, int>
    {
        public PurchaseDescriptor() : base("Purchase", "Id", nameof(Purchase.Id))
        {
        }
    }
}