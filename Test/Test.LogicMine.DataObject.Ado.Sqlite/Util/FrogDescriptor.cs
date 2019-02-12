using LogicMine.DataObject.Ado.Sqlite;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.DataObject.Ado.Sqlite.Util
{
    public class FrogDescriptor : SqliteMappedObjectDescriptor<Frog<int>, int>
    {
        public FrogDescriptor() : base("Frog", "FrogId", nameof(Frog<int>.Id))
        {
        }

        public override string GetMappedColumnName(string propertyName)
        {
            if (propertyName == nameof(Frog<int>.Id))
                return "FrogId";

            return base.GetMappedColumnName(propertyName);
        }
    }
}