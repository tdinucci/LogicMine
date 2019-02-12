using LogicMine.DataObject.Ado;
using LogicMine.DataObject.Ado.Sqlite;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Ado.Sqlite.Util
{
    public class FrogObjectStore : SqliteMappedObjectStore<Frog<int>, int>
    {
        private static readonly FrogDescriptor ObjDescriptor = new FrogDescriptor();

        public FrogObjectStore(string connectionString) :
            base(connectionString, ObjDescriptor, new DbMapper<Frog<int>>(ObjDescriptor))
        {
        }
    }
}