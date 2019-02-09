using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicMine.DataObject
{
    public class DataObjectDescriptorRegistry : IDataObjectDescriptorRegistry
    {
        private readonly Dictionary<string, IDataObjectDescriptor> _descriptors =
            new Dictionary<string, IDataObjectDescriptor>();

        public IDataObjectDescriptorRegistry Register(IDataObjectDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

            var registeredTypeName = descriptor.DataType.Name.ToLower();
            if (_descriptors.ContainsKey(registeredTypeName))
                throw new InvalidOperationException($"There is already a descriptor for '{descriptor.DataType.Name}'");

            _descriptors.Add(registeredTypeName, descriptor);

            return this;
        }

        public IDataObjectDescriptor GetDescriptor(Type dataType)
        {
            if (dataType == null) throw new ArgumentNullException(nameof(dataType));

            return GetDescriptor(dataType.Name);
        }

        public IDataObjectDescriptor GetDescriptor(string dataTypeName)
        {
            if (string.IsNullOrWhiteSpace(dataTypeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dataTypeName));

            if (!_descriptors.TryGetValue(dataTypeName.ToLower(), out var descriptor))
                throw new InvalidOperationException($"There is no descriptor registered for '{dataTypeName}'");

            return descriptor;
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

        public IEnumerable<Type> GetKnownDataTypes()
        {
            return _descriptors.Values.Select(d => d.DataType);
        }
    }
}