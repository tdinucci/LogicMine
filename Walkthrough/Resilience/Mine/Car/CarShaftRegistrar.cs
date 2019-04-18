using System;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Ado.Sqlite;

namespace Resilience.Mine.Car
{
    public class CarShaftRegistrar : DataObjectShaftRegistrar<Car, int>
    {
        private readonly DbConnectionString _connectionString;
        private readonly ITransientErrorAwareExecutor _transientErrorAwareExecutor;

        public CarShaftRegistrar(DbConnectionString connectionString,
            ITransientErrorAwareExecutor transientErrorAwareExecutor)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _transientErrorAwareExecutor = transientErrorAwareExecutor;
        }

        protected override IDataObjectStore<Car, int> GetDataObjectStore()
        {
            return new SqliteMappedObjectStore<Car, int>(_connectionString.Value, new CarDescriptor(), null,
                _transientErrorAwareExecutor);
        }

        protected override IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
        {
            return new Shaft<TRequest, TResponse>(terminal);
        }
    }
}