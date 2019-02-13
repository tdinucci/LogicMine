using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.CreateObject;
using Sample.LogicMine.Shop.Service.Mine.Purchase.Create;

namespace Sample.LogicMine.Shop.Service.Mine.Purchase
{
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
            return new SqliteMappedObjectStore<Purchase, int>(_dbConnectionString, new PurchaseDescriptor());
        }

        protected override IShaft<CreateObjectRequest<Purchase>, CreateObjectResponse<Purchase, int>>
            BuildCreateObjectShaft(IDataObjectStore<Purchase, int> objectStore)
        {
            return base.BuildCreateObjectShaft(objectStore)
                .AddToTop(
                    new ValidationStation(),
                    new PullDependantDataStation(),
                    new CalculatePriceStation());
        }
    }
}