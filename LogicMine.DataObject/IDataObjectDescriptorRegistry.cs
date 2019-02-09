using System;
using System.Collections.Generic;

namespace LogicMine.DataObject
{
    public interface IDataObjectDescriptorRegistry
    {
        IDataObjectDescriptorRegistry Register(IDataObjectDescriptor descriptor);

        IDataObjectDescriptor GetDescriptor(Type dataType);
        IDataObjectDescriptor GetDescriptor(string dataTypeName);
        TDescriptor GetDescriptor<T, TDescriptor>() where TDescriptor : IDataObjectDescriptor;

        IEnumerable<Type> GetKnownDataTypes();
    }
}