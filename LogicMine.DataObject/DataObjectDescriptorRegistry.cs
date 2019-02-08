using System;
using System.Collections.Generic;

namespace LogicMine.DataObject
{
    public class DataObjectDescriptorRegistry : IDataObjectDescriptorRegistry
    {
        private readonly Dictionary<Type, IDataObjectDescriptor> _descriptors =
            new Dictionary<Type, IDataObjectDescriptor>();

        public IDataObjectDescriptorRegistry Register(IDataObjectDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

            if (_descriptors.ContainsKey(descriptor.DataType))
                throw new InvalidOperationException($"There is already a descriptor for '{descriptor.DataType}'");

            _descriptors.Add(descriptor.DataType, descriptor);

            return this;
        }

        public IDataObjectDescriptor GetDescriptor(Type dataType)
        {
            if (dataType == null) throw new ArgumentNullException(nameof(dataType));

            if (!_descriptors.ContainsKey(dataType))
                throw new InvalidOperationException($"There is no descriptor registered for '{dataType}'");

            return _descriptors[dataType];
        }

        public TDescriptor GetDescriptor<T, TDescriptor>() where TDescriptor : IDataObjectDescriptor
        {
            var descriptor = GetDescriptor(typeof(T));
            if (!(descriptor is TDescriptor castDescriptor))
            {
                throw new InvalidOperationException(
                    $"A descriptor was found for '{typeof(T)}' but it was not a '{typeof(TDescriptor)}'");
            }

            return castDescriptor;
        }
    }
}