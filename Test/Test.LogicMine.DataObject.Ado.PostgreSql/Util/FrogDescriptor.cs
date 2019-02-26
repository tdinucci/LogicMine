using LogicMine.DataObject.Ado.PostgreSql;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Ado.PostgreSql.Util
{
    public class FrogDescriptor : PostgreSqlMappedObjectDescriptor<Frog<int>, int>
    {
        public FrogDescriptor() : base("frog", "id", nameof(Frog<int>.Id))
        {
        }
    }
}