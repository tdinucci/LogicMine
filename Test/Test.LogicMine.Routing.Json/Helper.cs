using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;

namespace Test.LogicMine.Routing.Json
{
    public class Helper
    {
        public IDataObjectDescriptorRegistry GetRegistry()
        {
            var registry = new DataObjectDescriptorRegistry();
            registry
                .Register(new MyFrogDescriptor());

            return registry;
        }
    }

    public class IntFrog : Frog<int>
    {
    }

    public class MyFrogDescriptor : DataObjectDescriptor<IntFrog, int>
    {
    }
}