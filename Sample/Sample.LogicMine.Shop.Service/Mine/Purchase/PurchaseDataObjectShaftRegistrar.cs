using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.CreateObject;
using Sample.LogicMine.Shop.Service.Mine.Purchase.Create;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase
{
    /// <summary>
    /// Here we specify the specialisations of our shafts that deal with the Purchase type.  Due to the inherited
    /// functionality we will have shafts for CRUD operations which ensure that requests are authorised.
    /// </summary>
    public class PurchaseDataObjectShaftRegistrar : DefaultDataObjectShaftRegistrar<Purchase, int>
    {
        private readonly string _dbConnectionString;

        public PurchaseDataObjectShaftRegistrar(DbConnectionString dbConnectionString, ITraceExporter traceExporter) :
            base(traceExporter)
        {
            if (dbConnectionString == null) throw new ArgumentNullException(nameof(dbConnectionString));

            _dbConnectionString = dbConnectionString.Value;
        }

        protected override IDataObjectStore<Purchase, int> GetDataObjectStore()
        {
            // return the store that contains purchases.  Here we're using an Sqlite store however this could easily 
            // be swapped out later for a different implementation of IDataObjectStore later.
            return new SqliteMappedObjectStore<Purchase, int>(_dbConnectionString, new PurchaseDescriptor());
        }

        protected override IShaft<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>>
            BuildCreateObjectShaft(IDataObjectStore<Purchase, int> objectStore)
        {
            // Here a few additional stations are added to the shaft.  They're being added to the bottom 
            // because we want the SecurityStation (which the base implementation adds) to remain at the top.
            return base.BuildCreateObjectShaft(objectStore)
                .AddToTop(
                    new ValidationStation(),
                    new PullDependantDataStation(),
                    new CalculatePriceStation());
        }
    }
}