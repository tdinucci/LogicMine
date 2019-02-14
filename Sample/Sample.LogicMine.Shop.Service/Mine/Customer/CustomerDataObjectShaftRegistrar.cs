using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using Sample.LogicMine.Shop.Service.Mine.Customer.Create;

namespace Sample.LogicMine.Shop.Service.Mine.Customer
{
    /// <summary>
    /// Here we specify the specialisations of our shafts that deal with the Customer type.  Due to the inherited
    /// functionality we will have shafts for CRUD operations which ensure that requests are authorised.
    /// </summary>
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
            // return the store that contains customers.  Here we're using an Sqlite store however this could easily 
            // be swapped out later for a different implementation of IDataObjectStore later.
            return new SqliteMappedObjectStore<Customer, int>(_dbConnectionString, new CustomerDescriptor());
        }

        protected override IShaft<CreateObjectRequest<Customer>, CreateObjectResponse<Customer, int>>
            BuildCreateObjectShaft(IDataObjectStore<Customer, int> objectStore)
        {
            // Here a couple of additional stations are added to the shaft.  They're being added to the bottom 
            // because we want the SecurityStation (which the base implementation adds) to remain at the top.
            return base.BuildCreateObjectShaft(objectStore)
                .AddToBottom(new ValidationStation(), new NormalisationStation());
        }

        protected override IShaft<DeleteObjectRequest<Customer, int>, DeleteObjectResponse> BuildDeleteObjectShaft(
            IDataObjectStore<Customer, int> objectStore)
        {
            // We don't want to allow for customers to be deleted, so prevent the base implementation from returning 
            // the default shaft.
            return null;
        }
    }
}