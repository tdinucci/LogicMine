using LogicMine.DataObject.Ado.Sqlite;

namespace Sample.LogicMine.Shop.Service.Mine.Product
{
    /// <summary>
    /// These descriptors contain information that allows for the mapping of .Net objects to a data store structure.
    /// In this sample the data type naturally maps to the underlying data store so very little has to be overriden.
    ///
    /// It's common for frameworks to handle this type of thing with attributes on your data types or by requiring
    /// data types to descend from special framework classes.  Neither of these are great because it couples your
    /// data types to the framework and most likely also to the underlying data store (i.e. your data types contains
    /// things like field names)
    /// </summary>
    public class ProductDescriptor : SqliteMappedObjectDescriptor<Product, int>
    {
        public ProductDescriptor() : base("Product", "Id", nameof(Product.Id))
        {
        }
    }
}