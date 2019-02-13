using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.UpdateObject;

namespace LogicMine.DataObject
{
    /// <summary>
    /// Represents a type that can register shafts within a mine.  This implementation is
    /// specialised for data objects and registers shafts for CRUD operations
    /// </summary>
    /// <typeparam name="T">The data type which the shafts will be registered for</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public abstract class DataObjectShaftRegistrar<T, TId> : ShaftRegistrar
        where T : class, new()
    {
        /// <summary>
        /// Get the object store which contains the T's
        /// </summary>
        /// <returns></returns>
        protected abstract IDataObjectStore<T, TId> GetDataObjectStore();

        /// <inheritdoc />
        public override void RegisterShafts(IMine mine)
        {
            AddStandardDataShafts(mine);
        }

        /// <summary>
        /// Returns a standard shaft.  For instance, if each shaft within a mine should include a station that enforces
        /// security then this method should return a shaft which includes the security enforcing station.
        /// </summary>
        /// <param name="terminal">The shaft terminal</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        protected abstract IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
            ITerminal<TRequest, TResponse> terminal)
            where TRequest : class, IRequest
            where TResponse : IResponse;
        
        protected virtual IShaft<GetObjectRequest<T, TId>, GetObjectResponse<T>> BuildGetObjectShaft(
            IDataObjectStore<T, TId> objectStore)
        {
            return GetBasicShaft(new GetObjectTerminal<T, TId>(objectStore));
        }

        protected virtual IShaft<GetCollectionRequest<T>, GetCollectionResponse<T>> BuildGetCollectionShaft(
            IDataObjectStore<T, TId> objectStore)
        {
            return GetBasicShaft(new GetCollectionTerminal<T>(objectStore));
        }

        protected virtual IShaft<CreateObjectRequest<T>, CreateObjectResponse<T, TId>> BuildCreateObjectShaft(
            IDataObjectStore<T, TId> objectStore)
        {
            return GetBasicShaft(new CreateObjectTerminal<T, TId>(objectStore));
        }

        protected virtual IShaft<UpdateObjectRequest<T, TId>, UpdateObjectResponse> BuildUpdateObjectShaft(
            IDataObjectStore<T, TId> objectStore)
        {
            return GetBasicShaft(new UpdateObjectTerminal<T, TId>(objectStore));
        }

        protected virtual IShaft<DeleteObjectRequest<T, TId>, DeleteObjectResponse> BuildDeleteObjectShaft(
            IDataObjectStore<T, TId> objectStore)
        {
            return GetBasicShaft(new DeleteObjectTerminal<T, TId>(objectStore));
        }

        private void AddStandardDataShafts(IMine mine)
        {
            var objectStore = GetDataObjectStore();

            var shafts = new IShaft[]
            {
                BuildGetObjectShaft(objectStore),
                BuildGetCollectionShaft(objectStore),
                BuildCreateObjectShaft(objectStore),
                BuildUpdateObjectShaft(objectStore),
                BuildDeleteObjectShaft(objectStore)
            };

            foreach (var shaft in shafts)
            {
                if (shaft != null)
                    mine.AddShaft(shaft);
            }
        }
    }
}