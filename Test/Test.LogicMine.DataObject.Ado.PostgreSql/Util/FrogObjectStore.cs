using LogicMine.DataObject.Ado;
using LogicMine.DataObject.Ado.PostgreSql;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Ado.PostgreSql.Util
{
    public class FrogObjectStore : PostgreSqlMappedObjectStore<Frog<int>, int>
    {
        private static readonly FrogDescriptor ObjDescriptor = new FrogDescriptor();

        public FrogObjectStore(string connectionString) :
            base(connectionString, ObjDescriptor, new DbMapper<Frog<int>>(ObjDescriptor))
        {
        }
    }
}