using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.UpdateObject;

namespace LogicMine.DataObject
{
    public abstract class DataObjectShaftRegistrar<T, TId> : ShaftRegistrar
        where T : class, new()
    {
        protected abstract IDataObjectStore<T, TId> GetDataObjectStore();

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