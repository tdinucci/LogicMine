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
                .Register(new MyFrogDescriptor())
                .Register(new MyAltFrogDescriptor());

            return registry;
        }

        private class IntFrog : Frog<int>
        {
        }

        private class MyFrogDescriptor : DataObjectDescriptor<IntFrog, int>
        {
        }

        private class MyAltFrogDescriptor : DataObjectDescriptor<AltFrog, int>
        {
        }
    }
}