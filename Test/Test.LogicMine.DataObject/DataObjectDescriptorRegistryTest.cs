using System;
using System.Linq;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Test.LogicMine.DataObject.Ado.Sqlite.Util;
using Xunit;

namespace Test.LogicMine.DataObject
{
    public class DataObjectDescriptorRegistryTest
    {
        [Fact]
        public void Register()
        {
            var registry = new DataObjectDescriptorRegistry();

            var frogDescriptor = new MyFrogDescriptor();
            var altFrogDescriptor = new MyAltFrogDescriptor();
            registry
                .Register(frogDescriptor)
                .Register(altFrogDescriptor);

            var readDesc = registry.GetDescriptor(typeof(IntFrog));
            Assert.Equal(frogDescriptor, readDesc);

            readDesc = registry.GetDescriptor("intFrog");
            Assert.Equal(frogDescriptor, readDesc);

            readDesc = registry.GetDescriptor("iNtFrOg");
            Assert.Equal(frogDescriptor, readDesc);

            readDesc = registry.GetDescriptor(typeof(AltFrog));
            Assert.Equal(altFrogDescriptor, readDesc);

            readDesc = registry.GetDescriptor("altFrog");
            Assert.Equal(altFrogDescriptor, readDesc);

            readDesc = registry.GetDescriptor("aLtFrOg");
            Assert.Equal(altFrogDescriptor, readDesc);

            var readGenDesc = registry.GetDescriptor<IntFrog, MyFrogDescriptor>();
            Assert.Equal(frogDescriptor, readGenDesc);

            var readAltGenDesc = registry.GetDescriptor<AltFrog, MyAltFrogDescriptor>();
            Assert.Equal(altFrogDescriptor, readAltGenDesc);
        }

        [Fact]
        public void RegisterDuplicate()
        {
            var registry = new DataObjectDescriptorRegistry()
                .Register(new MyFrogDescriptor())
                .Register(new MyAltFrogDescriptor());

            var ex = Assert.Throws<InvalidOperationException>(() => registry.Register(new MyFrogDescriptor()));
            Assert.Equal($"There is already a descriptor for '{typeof(IntFrog).Name}'", ex.Message);
        }

        [Fact]
        public void GetKnownTypes()
        {
            var registry = new DataObjectDescriptorRegistry();
            Assert.True(registry.GetKnownDataTypes() != null && !registry.GetKnownDataTypes().Any());

            registry.Register(new MyFrogDescriptor());

            Assert.True(registry.GetKnownDataTypes().Count() == 1);
            Assert.Contains(registry.GetKnownDataTypes(), t => t == typeof(IntFrog));

            registry.Register(new MyAltFrogDescriptor());

            Assert.True(registry.GetKnownDataTypes().Count() == 2);
            Assert.Contains(registry.GetKnownDataTypes(), t => t == typeof(IntFrog));
            Assert.Contains(registry.GetKnownDataTypes(), t => t == typeof(AltFrog));
        }

        [Fact]
        public void GetDescriptor_Bad()
        {
            var registry = new DataObjectDescriptorRegistry();
            
            Assert.Throws<ArgumentNullException>(() => registry.GetDescriptor((Type)null));
            Assert.Throws<ArgumentException>(() => registry.GetDescriptor((string)null));
            
            var ex = Assert.Throws<InvalidOperationException>(() => registry.GetDescriptor(typeof(Frog<int>)));
            Assert.Equal($"There is no descriptor registered for '{typeof(Frog<int>).Name}'", ex.Message);
            
            ex = Assert.Throws<InvalidOperationException>(() => registry.GetDescriptor("frog"));
            Assert.Equal($"There is no descriptor registered for 'frog'", ex.Message);
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