using System;

namespace LogicMine.DataObject
{
    public interface IDataObjectDescriptorRegistry
    {
        IDataObjectDescriptorRegistry Register(IDataObjectDescriptor descriptor);
        IDataObjectDescriptor GetDescriptor(Type dataType);
        TDescriptor GetDescriptor<T, TDescriptor>() where TDescriptor : IDataObjectDescriptor;
    }
}