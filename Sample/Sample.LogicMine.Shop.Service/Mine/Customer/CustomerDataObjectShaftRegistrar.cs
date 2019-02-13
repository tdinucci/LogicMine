using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using Sample.LogicMine.Shop.Service.Mine.Customer.Create;

namespace Sample.LogicMine.Shop.Service.Mine.Customer
{
    public class CustomerDataObjectShaftRegistrar : DefaultDataObjectShaftRegistrar<Customer, int>
    {
        private readonly string _dbConnectionString;

        public CustomerDataObjectShaftRegistrar(DbConnectionString dbConnectionString, ITraceExporter traceExporter) :
            base(traceExporter)
        {
            if (dbConnectionString == null) throw new ArgumentNullException(nameof(dbConnectionString));

            _dbConnectionString = dbConnectionString.Value;
        }

        protected override IDataObjectStore<Customer, int> GetDataObjectStore()
        {
            return new SqliteMappedObjectStore<Customer, int>(_dbConnectionString, new CustomerDescriptor());
        }

        protected override IShaft<CreateObjectRequest<Customer>, CreateObjectResponse<Customer, int>>
            BuildCreateObjectShaft(IDataObjectStore<Customer, int> objectStore)
        {
            return base.BuildCreateObjectShaft(objectStore)
                .AddToTop(new ValidationStation(), new NormalisationStation());
        }

        protected override IShaft<DeleteObjectRequest<Customer, int>, DeleteObjectResponse> BuildDeleteObjectShaft(
            IDataObjectStore<Customer, int> objectStore)
        {
            // disable the delete shaft - in a real application you'd most likely have a shaft and it would either
            // disallow deletion if a customer had made a purchase or perform a soft delete
            return null;
        }
    }
}