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

        private void AddStandardDataShafts(IMine mine)
        {
            var objectStore = GetDataObjectStore();

            mine
                .AddShaft(GetBasicShaft(new GetObjectTerminal<T, TId>(objectStore)))
                .AddShaft(GetBasicShaft(new GetCollectionTerminal<T>(objectStore)))
                .AddShaft(GetBasicShaft(new CreateObjectTerminal<T, TId>(objectStore)))
                .AddShaft(GetBasicShaft(new UpdateObjectTerminal<T, TId>(objectStore)))
                .AddShaft(GetBasicShaft(new DeleteObjectTerminal<T, TId>(objectStore)));
        }
    }
}