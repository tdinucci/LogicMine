using LogicMine.DataObject.Ado.Sqlite;

namespace Sample.LogicMine.Shop.Service.Mine.Product
{
    public class ProductDescriptor : SqliteMappedObjectDescriptor<Product, int>
    {
        public ProductDescriptor() : base("Product", "Id", nameof(Product.Id))
        {
        }
    }
}