using System;
using System.Linq;
using LogicMine.DataObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject
{
    public class DataObjectDescriptorTest
    {
        [Fact]
        public void Construct()
        {
            var descriptor = new FrogDescriptor();

            Assert.True(descriptor.GetReadableProperties().Count() == 2);
            Assert.Contains(nameof(Frog<int>.Id), descriptor.GetReadableProperties().Select(p => p.Name));
            Assert.Contains(nameof(Frog<int>.Name), descriptor.GetReadableProperties().Select(p => p.Name));

            Assert.Equal(typeof(Frog<int>), descriptor.DataType);
            Assert.Equal(typeof(int), descriptor.IdType);

            Assert.True(descriptor.CanRead(nameof(Frog<int>.Id)));
            Assert.True(descriptor.CanRead(nameof(Frog<int>.Name)));
            Assert.False(descriptor.CanRead(nameof(Frog<int>.DateOfBirth)));

            Assert.True(descriptor.CanWrite(nameof(Frog<int>.Id)));
            Assert.True(descriptor.CanWrite(nameof(Frog<int>.Name)));
            Assert.True(descriptor.CanWrite(nameof(Frog<int>.DateOfBirth)));
        }

        [Fact]
        public void Construct_ReadOnly()
        {
            var descriptor = new FrogDescriptor(nameof(Frog<int>.Id), nameof(Frog<int>.Name));

            Assert.True(descriptor.GetReadableProperties().Count() == 2);
            Assert.Contains(nameof(Frog<int>.Id), descriptor.GetReadableProperties().Select(p => p.Name));
            Assert.Contains(nameof(Frog<int>.Name), descriptor.GetReadableProperties().Select(p => p.Name));

            Assert.Equal(typeof(Frog<int>), descriptor.DataType);
            Assert.Equal(typeof(int), descriptor.IdType);

            Assert.True(descriptor.CanRead(nameof(Frog<int>.Id)));
            Assert.True(descriptor.CanRead(nameof(Frog<int>.Name)));
            Assert.False(descriptor.CanRead(nameof(Frog<int>.DateOfBirth)));

            Assert.False(descriptor.CanWrite(nameof(Frog<int>.Id)));
            Assert.False(descriptor.CanWrite(nameof(Frog<int>.Name)));
            Assert.True(descriptor.CanWrite(nameof(Frog<int>.DateOfBirth)));
        }

        [Fact]
        public void GetMappedColumnName()
        {
            var descriptor = new FrogDescriptor();

            Assert.Equal("MappedId", descriptor.GetMappedColumnName(nameof(Frog<int>.Id)));
            Assert.Equal("MappedName", descriptor.GetMappedColumnName(nameof(Frog<int>.Name)));
            Assert.Equal(nameof(Frog<int>.DateOfBirth), descriptor.GetMappedColumnName(nameof(Frog<int>.DateOfBirth)));

            var nonExistentProperty = "NonExistent";
            var ex = Assert.Throws<InvalidOperationException>(() =>
                descriptor.GetMappedColumnName(nonExistentProperty));
            Assert.Equal(
                $"There is no property called '{nonExistentProperty}' on '{typeof(Frog<int>)}' - case insensitive search",
                ex.Message);
        }

        [Fact]
        public void ProjectColumnValue()
        {
            var descriptor = new FrogDescriptor();

            var trueBoolString = "true";
            var falseBoolString = "false";
            var intString = "789";

            Assert.True((bool) descriptor.ProjectColumnValue(trueBoolString, typeof(bool)));
            Assert.False((bool) descriptor.ProjectColumnValue(falseBoolString, typeof(bool)));
            Assert.Equal(789, (int) descriptor.ProjectColumnValue(intString, typeof(int)));
            Assert.Null(descriptor.ProjectColumnValue(null, typeof(bool?)));

            Assert.True(descriptor.ProjectColumnValue<bool>(trueBoolString));
            Assert.False(descriptor.ProjectColumnValue<bool>(falseBoolString));
            Assert.Equal(789, descriptor.ProjectColumnValue<int>(intString));
            Assert.Null(descriptor.ProjectColumnValue<int?>(null));
        }

        [Fact]
        public void ProjectPropertyValue()
        {
            var descriptor = new FrogDescriptor();

            var date = DateTime.Now;
            var idString = "123";
            var dateString = date.ToLongDateString();

            Assert.Equal(123, (int) descriptor.ProjectPropertyValue(idString, nameof(Frog<int>.Id)));
            Assert.Equal(date.Date,
                ((DateTime) descriptor.ProjectPropertyValue(dateString, nameof(Frog<int>.DateOfBirth))).Date);
        }

        private class FrogDescriptor : DataObjectDescriptor<Frog<int>, int>
        {
            public FrogDescriptor(params string[] readOnlyPropertyNames) : base(readOnlyPropertyNames)
            {
            }

            public override bool CanRead(string propertyName)
            {
                return propertyName != nameof(Frog<int>.DateOfBirth);
            }

            public override string GetMappedColumnName(string propertyName)
            {
                switch (propertyName)
                {
                    case nameof(Frog<int>.Id):
                        return "MappedId";
                    case nameof(Frog<int>.Name):
                        return "MappedName";
                    default:
                        return base.GetMappedColumnName(propertyName);
                }
            }

            public override object ProjectColumnValue(object columnValue, Type propertyType)
            {
                if (columnValue != null)
                {
                    if ((propertyType == typeof(bool) || propertyType == typeof(bool?)) && !(columnValue is bool))
                        return Convert.ToBoolean(columnValue);
                    if ((propertyType == typeof(int) || propertyType == typeof(int?)) && !(columnValue is int))
                        return Convert.ToInt32(columnValue);
                }

                return base.ProjectColumnValue(columnValue, propertyType);
            }

            public override object ProjectPropertyValue(object propertyValue, string propertyName)
            {
                var propertyType = DataType.GetProperty(propertyName).PropertyType;
                if (propertyValue != null)
                {
                    if ((propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) &&
                        !(propertyValue is DateTime))
                    {
                        return Convert.ToDateTime(propertyValue);
                    }
                    if ((propertyType == typeof(int) || propertyType == typeof(int?)) && !(propertyValue is int))
                        return Convert.ToInt32(propertyValue);
                }

                return base.ProjectPropertyValue(propertyValue, propertyName);
            }
        }
    }
}