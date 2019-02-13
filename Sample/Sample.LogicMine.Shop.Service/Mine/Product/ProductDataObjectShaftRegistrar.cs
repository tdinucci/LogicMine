using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using Sample.LogicMine.Shop.Service.Mine.Product.Create;

namespace Sample.LogicMine.Shop.Service.Mine.Product
{
    public class ProductDataObjectShaftRegistrar : DefaultDataObjectShaftRegistrar<Product, int>
    {
        private readonly string _dbConnectionString;

        public ProductDataObjectShaftRegistrar(DbConnectionString dbConnectionString, ITraceExporter traceExporter) :
            base(traceExporter)
        {
            if (dbConnectionString == null) throw new ArgumentNullException(nameof(dbConnectionString));

            _dbConnectionString = dbConnectionString.Value;
        }

        protected override IDataObjectStore<Product, int> GetDataObjectStore()
        {
            return new SqliteMappedObjectStore<Product, int>(_dbConnectionString, new ProductDescriptor());
        }

        protected override IShaft<CreateObjectRequest<Product>, CreateObjectResponse<Product, int>>
            BuildCreateObjectShaft(IDataObjectStore<Product, int> objectStore)
        {
            return base.BuildCreateObjectShaft(objectStore)
                .AddToTop(new ValidationStation());
        }

        protected override IShaft<DeleteObjectRequest<Product, int>, DeleteObjectResponse> BuildDeleteObjectShaft(
            IDataObjectStore<Product, int> objectStore)
        {
            // disable the delete shaft - in a real application you'd most likely have a shaft and it would either
            // disallow deletion if a product had been purchased or perform a soft delete
            return null;
        }
    }
}