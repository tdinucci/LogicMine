using System;
using Database2.Mine.Car.Create;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateCollection;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.UpdateObject;

namespace Database2.Mine.Car
{
    public class CarShaftRegistrar : DataObjectShaftRegistrar<Car, int>
    {
        private readonly DbConnectionString _connectionString;

        public CarShaftRegistrar(DbConnectionString connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override IDataObjectStore<Car, int> GetDataObjectStore()
        {
            return new CarObjectStore(_connectionString.Value, new CarDescriptor());
        }

        protected override IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
        {
            return new Shaft<TRequest, TResponse>(terminal);
        }

        protected override IShaft<CreateObjectRequest<Car>, CreateObjectResponse<Car, int>> BuildCreateObjectShaft(
            IDataObjectStore<Car, int> objectStore)
        {
            return base.BuildCreateObjectShaft(objectStore)
                .AddToBottom(new ValidationStation());
        }

        protected override IShaft<CreateCollectionRequest<Car>, CreateCollectionResponse<Car>>
            BuildCreateCollectionShaft(IDataObjectStore<Car, int> objectStore)
        {
            return null;
        }

        protected override IShaft<UpdateObjectRequest<Car, int>, UpdateObjectResponse<Car, int>> BuildUpdateObjectShaft(
            IDataObjectStore<Car, int> objectStore)
        {
            return null;
        }
    }
}