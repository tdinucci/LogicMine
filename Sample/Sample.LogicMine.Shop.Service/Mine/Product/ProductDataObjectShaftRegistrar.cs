using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using Sample.LogicMine.Shop.Service.Mine.Product.Create;

namespace Sample.LogicMine.Shop.Service.Mine.Product
{
    /// <summary>
    /// Here we specify the specialisations of our shafts that deal with the Product type.  Due to the inherited
    /// functionality we will have shafts for CRUD operations which ensure that requests are authorised.
    /// </summary>
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
            // return the store that contains products.  Here we're using an Sqlite store however this could easily 
            // be swapped out later for a different implementation of IDataObjectStore later.
            return new SqliteMappedObjectStore<Product, int>(_dbConnectionString, new ProductDescriptor());
        }

        protected override IShaft<CreateObjectRequest<Product>, CreateObjectResponse<Product, int>>
            BuildCreateObjectShaft(IDataObjectStore<Product, int> objectStore)
        {
            // Here a new station is added to the shaft.  It's being added to the bottom because we want the
            // SecurityStation (which the base implementation adds) to remain at the top.
            return base.BuildCreateObjectShaft(objectStore)
                .AddToBottom(new ValidationStation());
        }

        protected override IShaft<DeleteObjectRequest<Product, int>, DeleteObjectResponse<Product, int>>
            BuildDeleteObjectShaft(IDataObjectStore<Product, int> objectStore)
        {
            // We don't want to allow for customers to be deleted, so prevent the base implementation from returning 
            // the default shaft.
            return null;
        }
    }
}